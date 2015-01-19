namespace Simplex2D
{
    using System.Windows.Forms;
    using Library;

    public partial class Render : Form
    {
        public Render(string inputFile)
        {
            InitializeComponent();

            var parser = new Parser(inputFile);
            //InitializeParser(parser);
            Shown += (sender, args) => parser.Start();
        }
    }
}
