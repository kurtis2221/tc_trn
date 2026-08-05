using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Media;
using System.IO;

namespace tc_trn
{
    public partial class Form1 : Form
    {
        //Y axis is used for height so Y and Z is swapped in memory read and write
        //Y axis works inverted
        const string GAME_PROC = "TrueCrime";
        const string GAME_EXE = GAME_PROC + ".exe";
        const string TRN_TELE = "tc_trn_tele.ini";
        const string TRN_SND = "tc_trn.wav";
        const char TRN_DELIM = '|';

        const float TELE_LIMIT = 50;
        const int TELE_INTERV = 150;

        static MemoryEdit.Memory mem;

        KeyHook.GlobalKeyboardHook gkh;
        Thread thd;

        Process game;
        HotKey[] hotkeys;
        List<Teleport> teleports;
        bool running = true;
        bool block_events = false;
        bool block_tele = false;
        bool game_active = false;

        //Vector zero crashed the game
        Vector3 saved_pos = new Vector3(1000, 1000, 0);
        float fly_speed;
        int tmr_interv;
        SoundPlayer snd;

        static uint ptr_player = 0x006D9578;
        static uint ptr_camera = 0x006D9570;
        static uint ptr_state = 0x006D9554;

        static uint offs_pos = 0x40;
        static uint offs_health = 0x1E0;
        static uint offs_ammo_l = 0x4A0; //0x4A4
        static uint offs_ammo_r = 0x280; //0x284
        static uint offs_anim = 0x80;
        //
        static uint offs_cam_pos = 0x40;
        static uint offs_state_fall = 0x344;

        static uint addr_badge = 0x006D5654;
        static uint addr_karma = 0x006D5658;

        static uint addr_noclip = 0x00401A5D;
        static uint addr_noclip2 = 0x00504B2D;
        static byte[] asm_noclip_nop = { 0x90, 0x90, 0x90, 0x90 };
        //Collision
        static byte[] asm_noclip_orig = { 0x0F, 0x29, 0x41, 0x40 }; //movaps [ecx+40],xmm0
        //Gravity
        static byte[] asm_noclip_orig2 = { 0x0F, 0x29, 0x4F, 0x30 }; //movaps [edi+30],xmm1
        //Animation
        static byte[] data_anim_idle = { 0x00, 0x56 }; //86

        static uint addr_player = 0x0;
        static uint addr_camera = 0x0;
        static uint addr_state = 0x0;

        public Form1()
        {
            InitializeComponent();
            InitTrainer();
        }

        private void InitTrainer()
        {
            //Invariant number format
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            //
            hotkeys = new HotKey[]
            {
                new HotKey(Keys.Shift, Keys.I, Cheat_FlyToggle, "Toggle NoClip"),
                new HotKey(Keys.Shift, Keys.P, Cheat_FlyForward, "NoClip Forward") { fly = true },
                new HotKey(Keys.Shift, Keys.O, Cheat_FlyUp, "NoClip Up") { fly = true },
                new HotKey(Keys.Shift, Keys.L, Cheat_FlyDown, "NoClip Down") { fly = true },
                new HotKey(Keys.Shift, Keys.U, Cheat_FlySpeedAdd, "Increase NoClip Speed"),
                new HotKey(Keys.Shift, Keys.J, Cheat_FlySpeedSub, "Decrease NoClip Speed"),
                new HotKey(Keys.Alt, Keys.D1, Cheat_AddKarma, "Add Karma"),
                new HotKey(Keys.Alt, Keys.D2, Cheat_AddKarma10, "Add 10 Karma"),
                new HotKey(Keys.Alt, Keys.D3, Cheat_SubKarma, "Sub Karma"),
                new HotKey(Keys.Alt, Keys.D4, Cheat_AddBadge, "Add Badge"),
                new HotKey(Keys.Alt, Keys.D5, Cheat_AddBadge10, "Add 10 Badge"),
                new HotKey(Keys.Alt, Keys.D6, Cheat_SubBadge, "Sub Badge"),
                new HotKey(Keys.Alt, Keys.D7, Cheat_Health, "Full Health"),
                new HotKey(Keys.Alt, Keys.D8, Cheat_Health, "Infinite Health") { toggle = true },
                new HotKey(Keys.Alt, Keys.D9, Cheat_Ammo, "Full Ammo"),
                new HotKey(Keys.Alt, Keys.D0, Cheat_Ammo, "Infinite Ammo") { toggle = true },
                new HotKey(Keys.Shift, Keys.D1, Cheat_SavePos, "Save Position"),
                new HotKey(Keys.Shift, Keys.D2, Cheat_LoadPos, "Load Position"),
                new HotKey(Keys.Shift, Keys.D3, Cheat_LoadPos2, "Teleport to Selected")
            };
            UpdateHotkeyText();
            teleports = new List<Teleport>();
            LoadTeleports();
            tmr_interv = 100;
            block_events = false;
            //Init hooks
            mem = new MemoryEdit.Memory();
            gkh = new KeyHook.GlobalKeyboardHook();
            gkh.KeyDown += gkh_KeyDown;
            gkh.KeyUp += gkh_KeyUp;
            //Set window controls
            tmr_interv = (int)nm_interv.Value;
            fly_speed = (int)nm_flyspeed.Value;
            //Start helper thread
            thd = new Thread(HelperThread);
            thd.Start();
            //Scan
            ScanForGame();
            try
            {
                if (File.Exists(TRN_SND))
                {
                    snd = new SoundPlayer(TRN_SND);
                    snd.Load();
                }
            }
            catch
            {
                snd = null;
            }
        }

        private void UpdateHotkeyText()
        {
            lb_hotkeys.Text = "Hotkeys\n";
            //Generate text
            foreach (HotKey h in hotkeys)
            {
                lb_hotkeys.Text += "\n" + h.text.PadRight(24, ' ') + " - " + h.mod + " + " + h.key;
            }
        }

        private void HelperThread()
        {
            while (running)
            {
                foreach (HotKey h in hotkeys)
                {
                    if ((h.fly || h.toggle) && h.active)
                    {
                        CallCheat(h.cheat);
                    }
                }
                Thread.Sleep(tmr_interv);
            }
        }

        private void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            if (!mem.IsFocused()) return;
            foreach (HotKey h in hotkeys)
            {
                if (e.Modifiers != h.mod || e.KeyCode != h.key) continue;
                if (h.active && !h.toggle) h.active = false;
            }
        }

        private void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (!mem.IsFocused()) return;
            foreach (HotKey h in hotkeys)
            {
                if (e.Modifiers != h.mod || e.KeyCode != h.key) continue;
                if (h.toggle)
                {
                    h.active = !h.active;
                    PlaySnd();
                }
                else if (!h.active)
                {
                    h.active = true;
                    if (!h.fly)
                    {
                        CallCheat(h.cheat);
                        PlaySnd();
                    }
                }
            }
        }

        private void LoadTeleports()
        {
            try
            {
                if (!File.Exists(TRN_TELE)) return;
                using (StreamReader sr = new StreamReader(TRN_TELE, Encoding.Default))
                {
                    while (sr.Peek() > -1)
                    {
                        string[] data = sr.ReadLine().Split(TRN_DELIM);
                        Teleport tele = new Teleport();
                        string name = data[0];
                        tele.name = name;
                        li_tele.Items.Add(name);
                        tele.pos = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
                        teleports.Add(tele);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBoxErr(ex.Message);
            }
        }

        private void SaveTeleports()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(TRN_TELE, false, Encoding.Default))
                {
                    foreach (Teleport tele in teleports)
                    {
                        sw.WriteLine(tele.name + TRN_DELIM + tele.pos.x + TRN_DELIM + tele.pos.y + TRN_DELIM + tele.pos.z);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBoxErr(ex.Message);
            }
        }

        private Vector3 GetPlayerPos()
        {
            uint pos_ptr = addr_player + offs_pos;
            return new Vector3
            (
                mem.ReadFloat(pos_ptr),
                mem.ReadFloat(pos_ptr + 0x8),
                mem.ReadFloat(pos_ptr + 0x4)
            );
        }

        private float GetPlayerZ()
        {
            uint pos_ptr = addr_player + offs_pos;
            return mem.ReadFloat(pos_ptr + 0x4);
        }

        private Vector3 GetCameraPos()
        {
            uint cam_ptr = addr_camera + offs_cam_pos;
            return new Vector3
            (
                mem.ReadFloat(cam_ptr),
                mem.ReadFloat(cam_ptr + 0x8),
                mem.ReadFloat(cam_ptr + 0x4)
            );
        }

        private void SetPlayerPos(Vector3 pos)
        {
            uint pos_ptr = addr_player + offs_pos;
            mem.WriteBytes(pos_ptr, BitConverter.GetBytes(pos.x), 4);
            mem.WriteBytes(pos_ptr + 0x8, BitConverter.GetBytes(pos.y), 4);
            mem.WriteBytes(pos_ptr + 0x4, BitConverter.GetBytes(pos.z), 4);
        }

        //For some reason the game crashes if the player or camera moves too fast
        private void SetPlayerPosEx(Vector3 pos)
        {
            if (block_tele) return;
            new Action(() =>
                {
                    block_tele = true;
                    uint pos_ptr = addr_player + offs_pos;
                    uint cam_ptr = addr_camera + offs_cam_pos;
                    float dist;
                    do
                    {
                        Vector3 curr_pos = GetPlayerPos();
                        float diff_x = pos.x - curr_pos.x;
                        float diff_y = pos.y - curr_pos.y;
                        dist = (float)Math.Sqrt((diff_x) * (diff_x) + (diff_y) * (diff_y));
                        if (diff_x < -TELE_LIMIT) diff_x = -TELE_LIMIT;
                        else if (diff_x > TELE_LIMIT) diff_x = TELE_LIMIT;
                        if (diff_y < -TELE_LIMIT) diff_y = -TELE_LIMIT;
                        else if (diff_y > TELE_LIMIT) diff_y = TELE_LIMIT;
                        curr_pos.x += diff_x;
                        curr_pos.y += diff_y;
                        mem.WriteBytes(pos_ptr, BitConverter.GetBytes(curr_pos.x), 4);
                        mem.WriteBytes(pos_ptr + 0x8, BitConverter.GetBytes(curr_pos.y), 4);
                        //Looks bad when the camera is adjusting
                        //mem.WriteBytes(cam_ptr, BitConverter.GetBytes(curr_pos.x), 4);
                        //mem.WriteBytes(cam_ptr + 0x8, BitConverter.GetBytes(curr_pos.y), 4);
                        ResetAnim();
                        Thread.Sleep(TELE_INTERV);
                    }
                    while (dist > TELE_LIMIT && game_active);
                    ResetAnim();
                    mem.WriteBytes(pos_ptr, BitConverter.GetBytes(pos.x), 4);
                    mem.WriteBytes(pos_ptr + 0x8, BitConverter.GetBytes(pos.y), 4);
                    mem.WriteBytes(pos_ptr + 0x4, BitConverter.GetBytes(pos.z), 4);
                    //Camera is too slow with long distance teleports, this has little to no effect
                    mem.WriteBytes(cam_ptr, BitConverter.GetBytes(pos.x), 4);
                    mem.WriteBytes(cam_ptr + 0x8, BitConverter.GetBytes(pos.y), 4);
                    mem.WriteBytes(cam_ptr + 0x4, BitConverter.GetBytes(pos.z), 4);
                    block_tele = false;
                }
            ).BeginInvoke(null, null);
        }

        private void SetPlayerZ(float z)
        {
            uint pos_ptr = addr_player + offs_pos;
            mem.WriteBytes(pos_ptr + 0x4, BitConverter.GetBytes(z), 4);
        }

        private void ResetAnim()
        {
            mem.WriteBytes(addr_player + offs_anim, data_anim_idle, 2);
        }

        private void Cheat_FlyToggle()
        {
            uint tmp = mem.Read(addr_noclip);
            if ((byte)tmp == 0x90)
            {
                mem.WriteBytes(addr_noclip, asm_noclip_orig, 4);
                mem.WriteBytes(addr_noclip2, asm_noclip_orig2, 4);
                mem.WriteBytes(addr_state + offs_state_fall, Constants.BYTE_0, 1);
                ResetAnim();
            }
            else
            {
                mem.WriteBytes(addr_noclip, asm_noclip_nop, 4);
                mem.WriteBytes(addr_noclip2, asm_noclip_nop, 4);
                mem.WriteBytes(addr_state + offs_state_fall, Constants.BYTE_1, 1);
                ResetAnim();
            }
        }

        private void Cheat_FlyForward()
        {
            Vector3 pl = GetPlayerPos();
            Vector3 cam = GetCameraPos();
            Vector3 diff = GetPosDiff(pl, cam);
            float angle = GetAngle(diff.x, diff.y);
            float dist = fly_speed;
            diff.x = (float)(dist * Math.Cos(angle));
            diff.y = (float)(dist * Math.Sin(angle));
            diff.z = 0;
            SetPlayerPos(GetPosAdd(pl, diff));
            ResetAnim();
        }

        private void Cheat_FlyUp()
        {
            float pz = GetPlayerZ();
            SetPlayerZ(pz - fly_speed);
            ResetAnim();
        }

        private void Cheat_FlyDown()
        {
            float pz = GetPlayerZ();
            SetPlayerZ(pz + fly_speed);
            ResetAnim();
        }

        private void Cheat_FlySpeedAdd()
        {
            if (fly_speed >= 20) return;
            fly_speed++;
            ChangeFlySpeed(fly_speed);
        }

        private void Cheat_FlySpeedSub()
        {
            if (fly_speed <= 1) return;
            fly_speed--;
            ChangeFlySpeed(fly_speed);
        }

        private void Cheat_AddKarma()
        {
            Cheat_Karma(1);
        }

        private void Cheat_AddKarma10()
        {
            Cheat_Karma(10);
        }

        private void Cheat_SubKarma()
        {
            Cheat_Karma(-1);
        }

        private void Cheat_AddBadge()
        {
            Cheat_Badge(1);
        }

        private void Cheat_AddBadge10()
        {
            Cheat_Badge(10);
        }

        private void Cheat_SubBadge()
        {
            Cheat_Badge(-1);
        }

        private void Cheat_Karma(int input)
        {
            int tmp = (int)mem.ReadByte(addr_karma) + input;
            byte[] data = BitConverter.GetBytes(tmp);
            mem.WriteBytes(addr_karma, data, 0x4);
        }

        private void Cheat_Badge(int input)
        {
            int tmp = (int)mem.ReadByte(addr_badge) + input;
            byte[] data = BitConverter.GetBytes(tmp);
            mem.WriteBytes(addr_badge, data, 0x4);
        }

        private void Cheat_Health()
        {
            mem.WriteBytes(addr_player + offs_health, Constants.BYTES_100, 0x4);
        }

        private void Cheat_Ammo()
        {
            uint addr = addr_player + offs_ammo_r;
            uint tmp = mem.Read(addr);
            byte[] data = BitConverter.GetBytes(tmp);
            addr += 0x4;
            mem.WriteBytes(addr, data, 0x4);
            //
            addr = addr_player + offs_ammo_l;
            tmp = mem.Read(addr);
            data = BitConverter.GetBytes(tmp);
            addr += 0x4;
            mem.WriteBytes(addr, data, 0x4);
        }

        private void Cheat_SavePos()
        {
            saved_pos = GetPlayerPos();
        }

        private void Cheat_LoadPos()
        {
            SetPlayerPosEx(saved_pos);
        }

        private void Cheat_LoadPos2()
        {
            int idx = li_tele.SelectedIndex;
            if (idx == -1) return;
            SetPlayerPosEx(teleports[idx].pos);
        }

        private void CallCheat(Action hndl)
        {
            addr_player = mem.Read(ptr_player);
            //Pointer is zero if not ingame
            if (addr_player == 0x0) return;
            addr_camera = mem.Read(ptr_camera);
            addr_state = mem.Read(ptr_state);
            hndl();
        }

        private Vector3 GetPosDiff(Vector3 p1, Vector3 p2)
        {
            p1.x -= p2.x;
            p1.y -= p2.y;
            p1.z -= p2.z;
            return p1;
        }

        private Vector3 GetPosAdd(Vector3 p1, Vector3 p2)
        {
            p1.x += p2.x;
            p1.y += p2.y;
            p1.z += p2.z;
            return p1;
        }

        private float GetAngle(float dx, float dy)
        {
            return (float)Math.Atan2(dy, dx);
        }

        private void ChangeFlySpeed(float fly_speed)
        {
            block_events = true;
            nm_flyspeed.Value = (decimal)fly_speed;
            block_events = false;
        }

        private void ScanForGame()
        {
            Process[] procs = Process.GetProcessesByName(GAME_PROC);
            if (procs.Length > 0)
            {
                game = procs[0];
                game_active = true;
                bt_start.Enabled = false;
                gkh.Hook();
                mem.Attach((uint)game.Id, MemoryEdit.Memory.ProcessAccessFlags.All);
            }
            else if (game_active)
            {
                game_active = false;
                bt_start.Enabled = true;
                gkh.Unhook();
            }
        }

        private void PlaySnd()
        {
            if (snd != null) snd.Play();
        }

        private void MsgBoxErr(string input)
        {
            MessageBox.Show(input, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void MsgBoxInfo(string input)
        {
            MessageBox.Show(input, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            running = false;
            bool thd_closed = false;
            try
            {
                thd_closed = thd.Join(2000);
            }
            catch { }
            if (!thd_closed) Environment.Exit(0);
        }

        private void tmr_scan_Tick(object sender, EventArgs e)
        {
            if (game == null || game.HasExited) ScanForGame();
        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(GAME_EXE))
                {
                    MsgBoxErr("Game exe not found!");
                    return;
                }
                Process.Start(GAME_EXE);
            }
            catch (Exception ex)
            {
                MsgBoxErr(ex.Message);
            }
        }

        private void bt_about_Click(object sender, EventArgs e)
        {
            MsgBoxInfo("Program written by Kurtis (2026)\nWritten in Visual C# 2008 Express Edition (.NET 3.5)");
        }

        private void nm_interv_ValueChanged(object sender, EventArgs e)
        {
            tmr_interv = (int)nm_interv.Value;
        }

        private void nm_flyspeed_ValueChanged(object sender, EventArgs e)
        {
            if (block_events) return;
            fly_speed = (int)nm_flyspeed.Value;
        }

        private void lb_tele_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = li_tele.SelectedIndex;
            if (idx == -1) return;
            Teleport tele = teleports[idx];
            tb_tele_name.Text = tele.name;
            Vector3 pos = teleports[idx].pos;
            lb_tele_coords.Text = pos.x + "\n" + pos.y + "\n" + pos.z;
        }

        private void bt_tele_add_Click(object sender, EventArgs e)
        {
            Teleport tele = new Teleport() { name = tb_tele_name.Text, pos = saved_pos };
            teleports.Add(tele);
            li_tele.Items.Add(tele.name);
        }

        private void bt_tele_upd_Click(object sender, EventArgs e)
        {
            int idx = li_tele.SelectedIndex;
            if (idx == -1) return;
            string name = tb_tele_name.Text;
            Teleport tele = teleports[idx];
            tele.name = name;
            Vector3 pos = saved_pos;
            tele.pos = pos;
            li_tele.Items[idx] = name;
            lb_tele_coords.Text = pos.x + "\n" + pos.y + "\n" + pos.z;
        }

        private void bt_tele_del_Click(object sender, EventArgs e)
        {
            int idx = li_tele.SelectedIndex;
            if (idx == -1) return;
            teleports.RemoveAt(idx);
            li_tele.Items.RemoveAt(idx);
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            SaveTeleports();
            MsgBoxInfo("Teleports file saved.");
        }

        private void tb_tele_name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == TRN_DELIM) e.Handled = true;
        }
    }
}
