using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsOut
{
    public partial class MainForm : Form
    {
        private int GRID_OFFSET = 25; // Distance from upper-left side of window
        private int GRID_LENGTH = 200; // Size in pixels of grid
        private int NUM_CELLS = 3; // Number of cells in grid
        int CELL_LENGTH;
        private bool[,] grid; // Stores on/off state of cells in grid
        private Random rand; // Used to generate random numbers
        private int INITIAL_Height;
        private int heightToAdd = 0;

        public MainForm()
        {
            InitializeComponent();

            INITIAL_Height = Size.Height;

            CELL_LENGTH = ((GRID_LENGTH +heightToAdd) / NUM_CELLS);
            rand = new Random(); // Initializes random number generator
            grid = new bool[NUM_CELLS, NUM_CELLS];
            // Turn entire grid on
            for (int r = 0; r < NUM_CELLS; r++)
                for (int c = 0; c < NUM_CELLS; c++)
                    grid[r, c] = true;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int r = 0; r < NUM_CELLS; r++)
                for (int c = 0; c < NUM_CELLS; c++)
                {
                    // Get proper pen and brush for on/off
                    // grid section
                    Brush brush;
                    Pen pen;
                    if (grid[r, c])
                    {
                        pen = Pens.Black;
                        brush = Brushes.White; // On
                    }
                    else
                    {
                        pen = Pens.White;
                        brush = Brushes.Black; // Off
                    }
                    // Determine (x,y) coord of row and col to draw rectangle
                    int x = c * CELL_LENGTH + GRID_OFFSET;
                    int y = r * CELL_LENGTH + GRID_OFFSET;
                    // Draw outline and inner rectangle
                    g.DrawRectangle(pen, x, y, CELL_LENGTH, CELL_LENGTH);
                    g.FillRectangle(brush, x + 1, y + 1, CELL_LENGTH - 1, CELL_LENGTH - 1);
                }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Make sure click was inside the grid
            if (e.X < GRID_OFFSET || e.X > CELL_LENGTH * NUM_CELLS + GRID_OFFSET ||
            e.Y < GRID_OFFSET || e.Y > CELL_LENGTH * NUM_CELLS + GRID_OFFSET)
                return;
            // Find row, col of mouse press
            int r = (e.Y - GRID_OFFSET) / CELL_LENGTH;
            int c = (e.X - GRID_OFFSET) / CELL_LENGTH;
            // Invert selected box and all surrounding boxes
            for (int i = r - 1; i <= r + 1; i++)
                for (int j = c - 1; j <= c + 1; j++)
                    if (i >= 0 && i < NUM_CELLS && j >= 0 && j < NUM_CELLS)
                        grid[i, j] = !grid[i, j];
            // Redraw grid
            this.Invalidate();
            // Check to see if puzzle has been solved
            if (PlayerWon())
            {
                // Display winner dialog box
                MessageBox.Show(this, "Congratulations! You've won!", "Lights Out!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool PlayerWon()
        {
            // Turn entire grid on
            for (int r = 0; r < NUM_CELLS; r++)
                for (int c = 0; c < NUM_CELLS; c++)
                    if (grid[r, c] == true)
                        return false;
            return true;
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            // Fill grid with either white or black
            for (int r = 0; r < NUM_CELLS; r++)
                for (int c = 0; c < NUM_CELLS; c++)
                    grid[r, c] = rand.Next(2) == 1;
            // Redraw grid
            this.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGameButton_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog(this);
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Determine if clicked menu item is the Blue menu item. 
            if (sender == x3ToolStripMenuItem)
            {
                // Set the checkmark for the x3ToolStripMenuItem menu item.
                x3ToolStripMenuItem.Checked = true;
                // Uncheck the x4ToolStripMenuItem and x5ToolStripMenuItem menu items.
                x4ToolStripMenuItem.Checked = false;
                x5ToolStripMenuItem.Checked = false;

                this.NUM_CELLS = 3;
            }
            else if (sender == x4ToolStripMenuItem)
            {
                // Set the checkmark for the x4ToolStripMenuItem menu item.
                x4ToolStripMenuItem.Checked = true;
                // Uncheck the x3ToolStripMenuItem and x5ToolStripMenuItem menu items.
                x3ToolStripMenuItem.Checked = false;
                x5ToolStripMenuItem.Checked = false;

                this.NUM_CELLS = 4;
            }
            else
            {
                // Set the checkmark for the x5ToolStripMenuItem.
                x5ToolStripMenuItem.Checked = true;
                // Uncheck the x4ToolStripMenuItem and x3ToolStripMenuItem menu items.
                x3ToolStripMenuItem.Checked = false;
                x4ToolStripMenuItem.Checked = false;

                this.NUM_CELLS = 5;
            }

            CELL_LENGTH = ((GRID_LENGTH + heightToAdd) / NUM_CELLS);
            rand = new Random(); // Initializes random number generator
            grid = new bool[NUM_CELLS, NUM_CELLS];
            // Turn entire grid on
            for (int r = 0; r < NUM_CELLS; r++)
                for (int c = 0; c < NUM_CELLS; c++)
                    grid[r, c] = true;

            newGameButton_Click(sender, e);

        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x3ToolStripMenuItem_Click(sender, e);
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x3ToolStripMenuItem_Click(sender, e);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            heightToAdd = Size.Height - INITIAL_Height; 
            CELL_LENGTH = ((GRID_LENGTH + heightToAdd) / NUM_CELLS);

            newGameButton.Location = new Point(newGameButton.Location.X, Size.Height - 70);
            exitButton.Location = new Point(exitButton.Location.X, Size.Height - 70);
            // Redraw grid
            this.Invalidate();
        }
    }
}

