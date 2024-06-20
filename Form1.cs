namespace GameBoyEmu
{
    public partial class Form1 : Form
    {
        private Graphics graphics;

        private CPU cpu;
        private Memory memory;
        private Cartridge cart;

        public Form1()
        {
            InitializeComponent();

            this.graphics = new Graphics();

            this.cpu = new CPU();
            this.cart = new Cartridge();
            this.memory = new Memory(this.cart);

            this.pictureBox1.Paint += PictureBox1_Paint;
        }

        private void PictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            System.Drawing.Graphics g = e.Graphics;

            this.graphics.Draw(g);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;

                MessageBox.Show(filePath);

                this.cart.LoadCartridge(filePath);
                this.memory.LoadMBCs();

                this.intitalizeWindow();
            }
        }

        private void intitalizeWindow()
        {
            this.pictureBox1.Invalidate();
        }

        private void gameLoop()
        {
            var running = true;

            new Thread(() =>
            {
                while (running)
                {
                    this.pictureBox1.Invalidate();
                }

                cleanUp();
            }).Start();
        }

        private void update()
        {
            const int GB_MAX_CYCLES = 4194304;
            const int FPS = 60;
            const int MAX_CYCLES = GB_MAX_CYCLES / FPS;

            int cyclesThisUpdate = 0;

            while (cyclesThisUpdate < MAX_CYCLES)
            {
                /*int cycles = ExecuteNextOpcode();
                cyclesThisUpdate += cycles;*/
            }
        }

        private void cleanUp()
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.pictureBox1.Invalidate();
        }
    }
}
