/**
 *  Go Applet
 *  1996.11		xinz	written in Java
 *  2001.3		xinz	port to C#
 *  2001.5.10	xinz	file parsing, back/forward
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics; 

namespace Go_WinApp
{

	public enum StoneColor : byte
	{
		Black = 0, White = 1
	}


	/**
	 * ZZ ZZZZZ
	 */
	public class GoBoard : System.Windows.Forms.Form
	{
		string [] strLabels; // {"Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z","Z"};

		int nSize;		                //ZZZZ ZZ ZZZ ZZZZZ, ZZZZZZZ ZZ 19
		const int nBoardMargin = 10;	//ZZZZZZ ZZ ZZZ ZZZZ ZZ ZZZ ZZZZZ
		int nCoodStart = 4;
		const int	nBoardOffset = 20;
		int nEdgeLen = nBoardOffset + nBoardMargin;
		int nTotalGridWidth = 360 + 36;	//ZZZ ZZZZZ ZZZZZ ZZ ZZZZZ ZZZZ
		int nUnitGridWidth = 22;		//ZZZ ZZZZZ ZZ ZZZZ ZZZZ
		int nSeq = 0;
		Rectangle rGrid;		    //ZZZ ZZZZ ZZZZ
		StoneColor m_colorToPlay;   //ZZZZZ ZZZZZ ZZZZZZ ZZZZ. 
		GoMove m_gmLastMove;	    //ZZZ ZZZZ ZZZZ, 
		Boolean bDrawMark;	        //ZZZZZZZ ZZ ZZZ ZZ ZZZZ ZZZ ZZZZ. 
		Boolean m_fAnyKill;	        //ZZZZZZZZ ZZZ ZZZZZZZZ ZZ ZZZ ZZZZ ZZZZ
		Spot [,] Grid;		        //ZZZZZ ZZZZ ZZ ZZZ ZZZZZ
		Pen penGrid;
		Brush brStar, brBoard, brBlack, brWhite, m_brMark;
	
        // ZZZZ ZZZZZZZZZ
        int nFFMove = 10;   //ZZ ZZZZ ZZZ ZZZZ 10 ZZZZZ. 
        //int nRewindMove = 10;  // ZZZZZZ; 

		GoTree	gameTree;

		/// <ZZZZZZZ>
		///    ZZZZZZZZ ZZZZZZZZ ZZZZZZZZ.
		/// </ZZZZZZZ>
		//private System.ComponentModel.Container components;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button Rewind;
		private System.Windows.Forms.Button FForward;
		private System.Windows.Forms.Button Save;
		private System.Windows.Forms.Button Open;
		private System.Windows.Forms.Button Back;
		private System.Windows.Forms.Button Forward;

		public GoBoard(int nSize)
		{
			//
			// ZZZZZZZZ ZZZ ZZZZZZZ ZZZZ ZZZZZZZZ ZZZZZZZ
			//
			InitializeComponent();

			//
			// ZZZZ: ZZZ ZZZ ZZZZZZZZZZZ ZZZZ ZZZZZ ZZZZZZZZZZZZZZZZZZZ ZZZZ
			//

			this.nSize = nSize;  //ZZZZZZZZZ ZZZZ.

			m_colorToPlay = StoneColor.Black;

			Grid = new Spot[nSize,nSize];
			for (int i=0; i<nSize; i++)
				for (int j=0; j<nSize; j++)
					Grid[i,j] = new Spot();
			penGrid = new Pen(Color.Brown, (float)0.5);
			//penStoneW = new Pen(Color.WhiteSmoke, (float)1);
			//penStoneB = new Pen(Color.Black,(float)1);
			//penMarkW = new Pen(Color.Blue, (float) 1);
			//penMarkB = new Pen(Color.Beige, (float) 1);

			brStar = new SolidBrush(Color.Black);
			brBoard = new SolidBrush(Color.Orange);
			brBlack = new SolidBrush(Color.Black);
			brWhite = new SolidBrush(Color.White);
			m_brMark = new SolidBrush(Color.Red);

			rGrid = new Rectangle(nEdgeLen, nEdgeLen,nTotalGridWidth, nTotalGridWidth);
			strLabels = new string [] {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t"};
			gameTree = new GoTree();
		}

		/// <ZZZZZZZ>
		///    ZZZZZZZZ ZZZZZZ ZZZ ZZZZZZZZ ZZZZZZZ - ZZ ZZZ ZZZZZZ
		///    ZZZ ZZZZZZZZ ZZ ZZZZ ZZZZZZ ZZZZ ZZZ ZZZZ ZZZZZZ.
		/// </ZZZZZZZ>
		private void InitializeComponent()
		{
            this.Open = new System.Windows.Forms.Button();
            this.Save = new System.Windows.Forms.Button();
            this.Rewind = new System.Windows.Forms.Button();
            this.Forward = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.FForward = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Open
            // 
            this.Open.Location = new System.Drawing.Point(534, 95);
            this.Open.Name = "Open";
            this.Open.Size = new System.Drawing.Size(67, 25);
            this.Open.TabIndex = 2;
            this.Open.Text = "open";
            this.Open.Click += new System.EventHandler(this.OpenClick);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(611, 95);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(67, 25);
            this.Save.TabIndex = 3;
            this.Save.Text = "save";
            this.Save.Click += new System.EventHandler(this.SaveClick);
            // 
            // Rewind
            // 
            this.Rewind.Location = new System.Drawing.Point(611, 60);
            this.Rewind.Name = "Rewind";
            this.Rewind.Size = new System.Drawing.Size(67, 25);
            this.Rewind.TabIndex = 5;
            this.Rewind.Text = "<<";
            this.Rewind.Click += new System.EventHandler(this.RewindClick);
            // 
            // Forward
            // 
            this.Forward.Location = new System.Drawing.Point(534, 26);
            this.Forward.Name = "Forward";
            this.Forward.Size = new System.Drawing.Size(67, 25);
            this.Forward.TabIndex = 0;
            this.Forward.Text = ">";
            this.Forward.Click += new System.EventHandler(this.ForwardClick);
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point(611, 26);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(67, 25);
            this.Back.TabIndex = 1;
            this.Back.Text = "<";
            this.Back.Click += new System.EventHandler(this.BackClick);
            // 
            // FForward
            // 
            this.FForward.Location = new System.Drawing.Point(534, 60);
            this.FForward.Name = "FForward";
            this.FForward.Size = new System.Drawing.Size(67, 25);
            this.FForward.TabIndex = 4;
            this.FForward.Text = ">>";
            this.FForward.Click += new System.EventHandler(this.FForwardClick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(536, 138);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(144, 335);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "please oepn a .sgf file to view, or just play on the board";
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1TextChanged);
            // 
            // GoBoard
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(685, 495);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Rewind);
            this.Controls.Add(this.FForward);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.Open);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.Forward);
            this.Name = "GoBoard";
            this.Text = "Go_WinForm";
            this.Click += new System.EventHandler(this.GoBoardClick);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintHandler);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpHandler);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		protected void TextBox1TextChanged (object sender, System.EventArgs e)
		{
			return;
		}

		private void PaintHandler(Object sender, PaintEventArgs e)
		{
			UpdateGoBoard(e);
		}

		protected void SaveClick (object sender, System.EventArgs e)
		{
			return;
		}

		protected void OpenClick (object sender, System.EventArgs e)
		{
			OpenFile();
			showGameInfo();
		}

		protected void RewindClick (object sender, System.EventArgs e)
		{
			gameTree.Reset();
			ResetBoard();
            showGameInfo();
		}

		protected void FForwardClick (object sender, System.EventArgs e)
		{
            if (gameTree != null)
            {
                int i = 0; 
                GoMove gm = null;
                for (gm = gameTree.DoNext(); gm != null; gm = gameTree.DoNext()) 
                {
                    PlayNext(gm);
                    if (i++ > nFFMove)
                        break; 
                }
            }
		}

		protected void ForwardClick (object sender, System.EventArgs e)
		{
			GoMove gm = gameTree.DoNext();
			if (null != gm)
			{
				PlayNext(gm);
			}
		}

		private void showGameInfo()
		{
			//ZZZZ ZZZ ZZZZZZZZZZZ ZZ ZZZZ ZZZZ, ZZ ZZZ
			textBox1.Clear();
			textBox1.AppendText(gameTree.Info);
		}

		protected void BackClick (object sender, System.EventArgs e)
		{
			GoMove gm = gameTree.DoPrevious();	//ZZZZ ZZZ ZZZZZ ZZZZ ZZZZZZ ZZZZZZZ ZZZZ
            if (null != gm)
            {
                PlayPrevious(gm);
            }
            else
            {
                ResetBoard();
                showGameInfo(); 
            }
		}

		Boolean onBoard(int x, int y) 
		{
			return (x>=0 && x<nSize && y>=0 && y<nSize);
		}

		protected void GoBoardClick (object sender, System.EventArgs e)
		{
			return;
		}

		private Point PointToGrid(int x, int y)
		{
			Point p= new Point(0,0);
			p.X = (x - rGrid.X + nUnitGridWidth/2) / nUnitGridWidth;
			p.Y = (y - rGrid.Y + nUnitGridWidth/2) / nUnitGridWidth;
			return p;
		}

		//ZZ ZZZ ZZ Z ZZZZZZZZ ZZZZZ (Z,Z) ZZ ZZZZZZ ZZZZZZ ZZZ ZZZZZZZZZ ZZ 
		//ZZ ZZZZZ ZZZZZ Z. (Z.Z. ZZZZZZ 1/3 ZZ ZZZZZZZZZZZZZZ
		private Boolean closeEnough(Point p, int x, int y)
		{
			if (x < rGrid.X+nUnitGridWidth*p.X-nUnitGridWidth/3 ||
				x > rGrid.X+nUnitGridWidth*p.X+nUnitGridWidth/3 ||
				y < rGrid.Y+nUnitGridWidth*p.Y-nUnitGridWidth/3 ||
				y > rGrid.Y+nUnitGridWidth*p.Y+nUnitGridWidth/3)
			{
				return false;
			}
			else 
				return true;
		}
        /// <ZZZZZZZ>
        /// 
        /// </ZZZZZZZ>
        /// <ZZZZZ ZZZZ="ZZZZZZ"></ZZZZZ>
        /// <ZZZZZ ZZZZ="Z"></ZZZZZ>
		private void MouseUpHandler(Object sender,MouseEventArgs e)
		{
			Point p;
			GoMove	gmThisMove;

			p = PointToGrid(e.X,e.Y);
			if (!onBoard(p.X, p.Y) || !closeEnough(p,e.X, e.Y)|| Grid[p.X,p.Y].HasStone())
				return; //ZZZZZZ ZZZZ Z ZZZZ ZZZZZ.

			//ZZZZZ ZZZZ ZZ Z ZZZZZZ ZZZZ, ZZ ZZZZ ZZ ZZZ ZZZ ZZZZ ZZ ZZZ ZZZZ ZZZZ
			gmThisMove = new GoMove(p.X, p.Y, m_colorToPlay, 0);
			PlayNext(gmThisMove);
			gameTree.AddMove(gmThisMove);
		}

		public void PlayNext(GoMove move) 
		{
			Point p = move.Point;
			m_colorToPlay = move.Color;	//ZZZ ZZZZZ, ZZZZZZZZZ ZZZZ ZZZZZZ ZZZZ ZZZZ ZZZZ.

			//ZZZZZ ZZZ ZZZZZ/ZZZZZZ ZZ ZZZZZZZ ZZZZZZZZZ
			clearLabelsAndMarksOnBoard(); 
			
			if (m_gmLastMove != null)
				repaintOneSpotNow(m_gmLastMove.Point);

			bDrawMark = true;
			Grid[p.X,p.Y].SetStone(move.Color);
			m_gmLastMove = new GoMove(p.X, p.Y, move.Color, nSeq++);
			//ZZZ ZZZZZ/ZZZZ
			setLabelsOnBoard(move);
			setMarksOnBoard(move);
			
			doDeadGroup(nextTurn(m_colorToPlay));
			//ZZ ZZZ ZZZZZ ZZZ ZZZZ, ZZ ZZZZ ZZ ZZZZZZZZ ZZZZ, ZZ ZZZZ ZZ ZZZ ZZZZZZZ ZZZZ ZZ ZZZZZZZZ. 
			if (m_fAnyKill)
				appendDeadGroup(ref move, nextTurn(m_colorToPlay));
			else //ZZZZ ZZ ZZZ ZZ ZZ'Z Z ZZZZZZZ
			{
				doDeadGroup(m_colorToPlay);
				if (m_fAnyKill)
					appendDeadGroup(ref move, m_colorToPlay);
			}
			m_fAnyKill = false;
			
			optRepaint();

			//ZZZZZZ ZZZZZ
			m_colorToPlay = nextTurn(m_colorToPlay);
			
			//ZZZZ ZZZ ZZZZZZZ, ZZ ZZZ
			textBox1.Clear();
			textBox1.AppendText(move.Comment);
		}

		private void appendDeadGroup(ref GoMove gm, StoneColor c)
		{
			ArrayList a = new ArrayList();
			for (int i=0; i<nSize; i++)
				for (int j=0; j<nSize; j++)
					if (Grid[i,j].IsKilled())
					{
						Point pt = new Point(i,j);
						a.Add(pt);
						Grid[i,j].SetNoKilled();
					}
			gm.DeadGroup = a;
			gm.DeadGroupColor = c;
		}

		public void ResetBoard()
		{
			int i,j;
			for (i=0; i<nSize; i++)
				for (j=0; j<nSize; j++) 
					Grid[i,j].RemoveStone();
			m_gmLastMove = null;
			Invalidate(null);
		}

		/*
		 * ZZZZ ZZZ ZZZZ ZZ ZZZZ ZZZ ZZZZ ZZZZZZZZZ ZZ ZZZZ ZZZZZZ ZZZZ ZZZZ ZZ ZZZZZZ. 
		 * ZZZZ ZZ ZZ:
		 * 	1. ZZZZZZ ZZZ ZZZZZZZ ZZZZ ZZZZ ZZZ ZZZZZ
		 *  1.1 ZZZZ ZZZZZZ ZZZ "ZZZZZZZZ" ZZZZZZZZZZ
		 *	2. store the stones got killed by current move
		 *  3. ZZZZZZZZZZ ZZZ ZZZ "ZZZZZZZZ"
		 */
		public void PlayPrevious(GoMove move)
		{
            if (m_gmLastMove != null)
                repaintOneSpotNow(m_gmLastMove.Point);
            
            m_colorToPlay = move.Color;

            Grid[move.Point.X,move.Point.Y].RemoveStone();

            if (null != move.DeadGroup)
            {
                System.Collections.IEnumerator ie = move.DeadGroup.GetEnumerator();

                Point deadp;
                while (ie.MoveNext())
                {
                    deadp = (Point)ie.Current;
                    Grid[deadp.X, deadp.Y].SetStone(move.DeadGroupColor);
                }
            }
            
            bDrawMark = true;
            m_gmLastMove = gameTree.PeekPrevious();
            
            if(null != m_gmLastMove)
                Grid[m_gmLastMove.Point.X, m_gmLastMove.Point.Y].SetUpdated();

            optRepaint();
			
			textBox1.Clear();
			textBox1.AppendText(move.Comment);
        }

				
		
		Rectangle getUpdatedArea(int i, int j) 
		{
			int x = rGrid.X + i * nUnitGridWidth - nUnitGridWidth/2;
			int y = rGrid.Y + j * nUnitGridWidth - nUnitGridWidth/2;
			return new Rectangle(x,y, nUnitGridWidth, nUnitGridWidth);
		}

		/**
		 * ZZZZZZZZ ZZZ ZZZZZZZ ZZZZ, ZZZZ ZZZZZZZ ZZZ ZZZZZZZ ZZZZZ ZZ ZZZ ZZZZZ
		 */
		private void optRepaint()
		{
			Rectangle r = new Rectangle(0,0,0,0);
			Region	re;

			for (int i=0; i<nSize; i++)
				for (int j=0; j<nSize; j++)
					if (Grid[i,j].IsUpdated()) 
					{
						r = getUpdatedArea(i,j);
						re = new Region(r);
						Invalidate(re);
					}
		}

		/*
		 * ZZZZZZZ ZZZZ ZZZ ZZZZ ZZ ZZZ ZZZZZ ZZZZZZZ 
		 */
		void repaintOneSpotNow(Point p)
		{
			Grid[p.X, p.Y].SetUpdated();
			bDrawMark = false;
			Rectangle r = getUpdatedArea(p.X, p.Y);
			Invalidate( new Region(r));
			Grid[p.X, p.Y].ResetUpdated();
			bDrawMark = true;
		}

		//ZZZZ ZZ ZZZZZZZZ ZZ ZZZZZZZ ZZZ ZZZ ZZZZ ZZZZ ZZZZ.  
		void recordMove(Point p, StoneColor colorToPlay) 
		{
			Grid[p.X,p.Y].SetStone(colorToPlay);
			// ZZZZZZ ZZZZ ZZZZ.
			m_gmLastMove = new GoMove(p.X, p.Y, colorToPlay, nSeq++);
		}

		static StoneColor  nextTurn(StoneColor c) 
		{
			if (c == StoneColor.Black)
				return StoneColor.White;
			else 
				return StoneColor.Black;
		}

		/**
		 *	bury the dead stones in a group (same color). 
		 *	if a stone in one group is dead, the whole group is dead.
		*/
		void buryTheDead(int i, int j, StoneColor c) 
		{
			if (onBoard(i,j) && Grid[i,j].HasStone() && 
				Grid[i,j].Color() == c) 
			{
				Grid[i,j].Die();
				buryTheDead(i-1, j, c);
				buryTheDead(i+1, j, c);
				buryTheDead(i, j-1, c);
				buryTheDead(i, j+1, c);
			}
		}

		void cleanScanStatus()
		{
			int i,j;
			for (i=0; i<nSize; i++)
				for (j=0; j<nSize; j++) 
					Grid[i,j].ClearScanned();
		}

		/**
		 * ZZZZZZ ZZZ ZZZZ ZZZZZ ZZZ ZZZZZ ZZZZ ZZZZ ZZZ ZZZZZ.
		 */
		void doDeadGroup(StoneColor c) 
		{
			int i,j;
			for (i=0; i<nSize; i++)
				for (j=0; j<nSize; j++) 
					if (Grid[i,j].HasStone() &&
						Grid[i,j].Color() == c) 
					{
						if (calcLiberty(i,j,c) == 0)
						{
							buryTheDead(i,j,c);
							m_fAnyKill = true;
						}
						cleanScanStatus();
					}
		}


		/**
		 * ZZZZZZZZZ ZZZ ZZZZZZZ ZZ ZZZ ZZZZZ, ZZZZZZZZ ZZZZ ZZZ ZZZZZ.
		 */
		int calcLiberty(int x, int y, StoneColor c) 
		{
			int lib = 0; // ZZZZZZZ	
			
			if (!onBoard(x,y))
				return 0;
			if (Grid[x,y].IsScanned())
				return 0;

			if (Grid[x,y].HasStone()) 
			{
				if (Grid[x,y].Color() == c) 
				{
					//ZZZ ZZZZZZZZZZ ZZZ ZZZZZZZ ZZZZZ.
					Grid[x,y].SetScanned();
					lib += calcLiberty(x-1, y, c);
					lib += calcLiberty(x+1, y, c);
					lib += calcLiberty(x, y-1, c);
					lib += calcLiberty(x, y+1, c);
				} 
				else 
					return 0;
			} 
			else 
			{// ZZZZ ZZ ZZZZZ ZZZ ZZZZZZZZZ. 
				lib ++;
				Grid[x,y].SetScanned();
			}

			return lib;
		}


		/**
		 * ZZZZ ZZZ ZZZZ ZZZZ
		 */
		void markLastMove(Graphics g) 
		{
			Brush brMark;
			if (m_gmLastMove.Color == StoneColor.White)
				brMark = brBlack;
			else 
				brMark = brWhite;
			Point p = m_gmLastMove.Point;
			g.FillRectangle( brMark,
				rGrid.X + (p.X) * nUnitGridWidth - (nUnitGridWidth-1)/8, 
				rGrid.Y + (p.Y) * nUnitGridWidth - (nUnitGridWidth-1)/8,
				3, 3);
		}

		private void clearLabelsAndMarksOnBoard()
		{
			for (int i=0; i<nSize; i++)
				for (int j=0; j<nSize; j++)
				{
					if (Grid[i,j].HasLabel())
						Grid[i,j].ResetLabel();
					if (Grid[i,j].HasMark())
						Grid[i,j].ResetMark();
				}

		}

		private void setLabelsOnBoard(GoMove gm)
		{
			short	nLabel = 0;
			Point p;
			if (null != gm.Labels)
			{
				//int i = gm.Labels.Count;
				//i = gm.Labels.Capacity;

				System.Collections.IEnumerator myEnumerator = gm.Labels.GetEnumerator();
				while (myEnumerator.MoveNext())
				{
					p = (Point)myEnumerator.Current;
					Grid[p.X,p.Y].SetLabel(++nLabel);
				}
			}
		}

		private void setMarksOnBoard(GoMove gm)
		{
			Point p;
			if (null != gm.Labels)
			{
				System.Collections.IEnumerator myEnumerator = gm.Marks.GetEnumerator();
				while ( myEnumerator.MoveNext() )
				{
					p = (Point)myEnumerator.Current;
					Grid[p.X,p.Y].SetMark();
				}
			}
		}

		private static Point SwapXY(Point p)
		{
			Point pNew = new Point(p.Y,p.X);
			return pNew;
		}

		private void DrawBoard(Graphics g)
		{
			//ZZZZZ ZZZ ZZZ ZZZZ ZZZ ZZZZZZZZZZZ
			string[] strV= {"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19"};
			string [] strH= {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T"};

			Point p1 = new Point(nEdgeLen,nEdgeLen);
			Point p2 = new Point(nTotalGridWidth+nEdgeLen,nEdgeLen);
			g.FillRectangle(brBoard,nBoardOffset,nBoardOffset,nTotalGridWidth+nBoardOffset,nTotalGridWidth+nBoardOffset);
			for (int i=0;i<nSize; i++)
			{
				g.DrawString(strV[i],this.Font, brBlack, 0, nCoodStart+ nBoardOffset + nUnitGridWidth*i);
				g.DrawString(strH[i],this.Font, brBlack, nBoardOffset + nCoodStart + nUnitGridWidth*i, 0);
				g.DrawLine(penGrid, p1, p2);
				g.DrawLine(penGrid, SwapXY(p1), SwapXY(p2));

				p1.Y += nUnitGridWidth;
				p2.Y += nUnitGridWidth;
			}
			//ZZZZ ZZZ ZZZZ ZZ ZZZ ZZZZZ
			Pen penHi = new Pen(Color.WhiteSmoke, (float)0.5);
			Pen penLow = new Pen(Color.Gray, (float)0.5);

			g.DrawLine(penHi, nBoardOffset, nBoardOffset, nTotalGridWidth+2*nBoardOffset, nBoardOffset);
			g.DrawLine(penHi, nBoardOffset, nBoardOffset, nBoardOffset, nTotalGridWidth+2*nBoardOffset);
			g.DrawLine(penLow, nTotalGridWidth+2*nBoardOffset,nTotalGridWidth+2*nBoardOffset, nBoardOffset+1, nTotalGridWidth+2*nBoardOffset);
			g.DrawLine(penLow, nTotalGridWidth+2*nBoardOffset,nTotalGridWidth+2*nBoardOffset, nTotalGridWidth+2*nBoardOffset, nBoardOffset+1);
		}

		void UpdateGoBoard(PaintEventArgs e)
		{
			DrawBoard(e.Graphics);
			
			//ZZZZ ZZZZ-ZZZZZ. 
			drawStars(e.Graphics);

			//ZZZZ ZZZZZZ
			drawEverySpot(e.Graphics);
		}

		//ZZZZ ZZZ ZZZZ ZZ ZZZZZZZZ ZZZZZZZZ
		void drawStar(Graphics g, int row, int col) 
		{
			g.FillRectangle(brStar,
				rGrid.X + (row-1) * nUnitGridWidth - 1, 
				rGrid.Y + (col-1) * nUnitGridWidth - 1, 
				3, 
				3);
		}

		//ZZZZ 9 ZZZZZ ZZZ ZZZZZZZ ZZZZ 19Z19. 
		void  drawStars(Graphics g)
		{
			drawStar(g, 4, 4);
			drawStar(g, 4, 10);
			drawStar(g, 4, 16);
			drawStar(g, 10, 4);
			drawStar(g, 10, 10);
			drawStar(g, 10, 16);
			drawStar(g, 16, 4);
			drawStar(g, 16, 10);
			drawStar(g, 16, 16);
		}

		/**
		 * ZZZZ Z ZZZZZ (ZZZZZ/ZZZZZ) ZZ ZZZZZZZZ ZZZZZZZZ.
		 */
		void drawStone(Graphics g, int row, int col, StoneColor c) 
		{
			Brush br;
			if (c == StoneColor.White)
				br = brWhite;
			else 
				br = brBlack;
			
			Rectangle r = new Rectangle(rGrid.X+ (row) * nUnitGridWidth - (nUnitGridWidth-1)/2, 
				rGrid.Y + (col) * nUnitGridWidth - (nUnitGridWidth-1)/2,
				nUnitGridWidth-1,
				nUnitGridWidth-1);

			g.FillEllipse(br, r);
		}

		void drawLabel(Graphics g, int x, int y, short nLabel) 
		{
			if (nLabel ==0)
				return;
			nLabel --;
			nLabel %= 18;			//ZZZZZZZZ ZZZZZ.

			//ZZZZZ ZZZ ZZ
			Rectangle r = new Rectangle(rGrid.X+ x * nUnitGridWidth - (nUnitGridWidth-1)/2, 
				rGrid.Y + y * nUnitGridWidth - (nUnitGridWidth-1)/2,
				nUnitGridWidth-1,
				nUnitGridWidth-1);

			g.FillEllipse(brBoard, r);

			g.DrawString(strLabels[nLabel],	//ZZZZZZ ZZZZZZ ZZZZ 1, ZZZ ZZZ ZZZZZZ ZZZZZZ ZZZZ 0.
				this.Font, 
				brBlack, 
				rGrid.X+ (x) * nUnitGridWidth - (nUnitGridWidth-1)/4, 
				rGrid.Y + (y) * nUnitGridWidth - (nUnitGridWidth-1)/2);
		}

		void drawMark(Graphics g, int x, int y)
		{
			g.FillRectangle( m_brMark,
				rGrid.X + x* nUnitGridWidth - (nUnitGridWidth-1)/8, 
				rGrid.Y + y * nUnitGridWidth - (nUnitGridWidth-1)/8,
				5, 5);
		}

		void drawEverySpot(Graphics g) 
		{
			for (int i=0; i<nSize; i++)
				for (int j=0; j<nSize; j++)
				{
					if (Grid[i,j].HasStone())
						drawStone(g, i, j, Grid[i,j].Color());
					if (Grid[i,j].HasLabel())
						drawLabel(g, i, j, Grid[i,j].GetLabel());
					if (Grid[i,j].HasMark())
						drawMark(g, i, j);
				}
			//ZZZZZZZZZ ZZZZ ZZZZ. 
			if (bDrawMark && m_gmLastMove != null)
				markLastMove(g);
		}

		//ZZZZ Z ZZZZ ZZZZ
		private void OpenFile()
		{
			OpenFileDialog openDlg = new OpenFileDialog();
			openDlg.Filter  = "sgf files (*.sgf)|*.sgf|All Files (*.*)|*.*";
			openDlg.FileName = "" ;
			openDlg.DefaultExt = ".sgf";
			openDlg.CheckFileExists = true;
			openDlg.CheckPathExists = true;
			
			DialogResult res = openDlg.ShowDialog ();
			
			if(res == DialogResult.OK)
			{
				if( !(openDlg.FileName).EndsWith(".sgf") && !(openDlg.FileName).EndsWith(".SGF")) 
					MessageBox.Show("Unexpected file format","Super Go Format",MessageBoxButtons.OK);
				else
				{
					FileStream f = new FileStream(openDlg.FileName, FileMode.Open); 
					StreamReader r = new StreamReader(f);
					string s = r.ReadToEnd();
					gameTree = new GoTree(s);
					gameTree.Reset();
                    ResetBoard();
					r.Close(); 
					f.Close();
				}
			}		
		}	
	}

	public class GoTest
	{
		/// <ZZZZZZZ>
		/// ZZZ ZZZZ ZZZZZ ZZZZZ ZZZ ZZZ ZZZZZZZZZZZ.
		/// </ZZZZZZZ>
        [STAThread]
		public static void Main() 
		{
			Application.Run(new GoBoard(19));
		}
	}

	
	//ZZZZ ZZZZZZZZZZZZ ZZ ZZZ ZZZZ ZZ ZZZ ZZZZZ
	public class Spot 
	{
		private Boolean bEmpty;
		private Boolean bKilled;
		private Stone s;
		private short	m_nLabel;
		private Boolean m_bMark;
		private Boolean bScanned;
		private Boolean bUpdated; //ZZ ZZZ ZZZZ ZZ ZZZZZZZ. (ZZZ ZZZZ/ZZZZ ZZZZZ/ZZZZZZ ZZZZZ)
		/**
		 * ZZZZZZZZZZZ.
		 */
		public Spot() 
		{
			bEmpty = true;
			bScanned = false;
			bUpdated = false;
			bKilled = false;
		}
		
		public Boolean HasStone() { return !bEmpty;	}
		public Boolean IsEmpty() {	return bEmpty;	}
		public Stone ThisStone() {	return s;}
		public StoneColor Color() {	return s.color;}

		public Boolean HasLabel() {return m_nLabel>0;}
		public Boolean HasMark() {return m_bMark;}
		public void SetLabel(short len) {m_nLabel = len; bUpdated = true; }
		public void SetMark() {m_bMark = true; bUpdated = true;}
		public void ResetLabel() {m_nLabel = 0; bUpdated = true;}
		public void ResetMark() {m_bMark = false; bUpdated = true;}
		public short	GetLabel() {return m_nLabel;}

		public Boolean IsScanned() { return bScanned;}
		public void SetScanned() {	bScanned = true;}
		public void ClearScanned() { bScanned = false; }

		public void SetStone(StoneColor color) 
		{
			if (bEmpty) 
			{
				bEmpty = false;
				s.color = color;
				bUpdated = true;
			} // ZZZZ ZZZZZZ ZZZZZZZZ. 
		}

		/*
		 * ZZZZZZ Z ZZZZZ ZZZZ ZZZ ZZZZZZZZ
		*/
		public void RemoveStone()
		{	//ZZZZZZ ZZZZZZ !ZZZZZZ
			bEmpty = true;
			bUpdated = true;
		}
				
		//ZZ ZZZZ ZZZZ ZZZZZ ZZZZZZZZZZZZZ ZZZZZZ ZZZ ZZZZ ZZZZZ Z.
		public void Die() 
		{
			bKilled = true;
			bEmpty = true;
			bUpdated = true;
		} 

		public Boolean IsKilled() { return bKilled;}
		public void SetNoKilled() { bKilled = false;}

		public void ResetUpdated() { bUpdated = false; bKilled = false;}

		//ZZ ZZZ ZZZZZZZ ZZZZZZZZZ ZZZZ ZZZZZZ (ZZZZZZZ)? 
		public Boolean IsUpdated() 
		{ 
			if (bUpdated)
			{	//ZZZZ ZZ ZZZZ ZZZ ZZZZZZ ZZZZZZZZZ ZZZ ZZZ ZZZZ ZZZZZZ
				bUpdated = false;
				return true;
			} 
			else 
				return false;
		}

		// ZZZZZ Z ZZZZ ZZ ZZ ZZZZZZZZZ, ZZZZ ZZ ZZZZZZZZ ZZZ ZZZZZ ZZZZ.
		public void SetUpdated() { bUpdated = true; }
	}

	/**
	 * Z ZZZZ ZZ Z ZZ ZZZZ.
	 */
	public class GoMove 
	{
		StoneColor m_c;	//ZZZZZ/ZZZZZ
		Point m_pos;		//ZZZZZZZZZZZ ZZ ZZZ ZZZZ.
		int m_n;			//ZZZZZZZZ ZZ ZZZ ZZZZZZZZZ.
		String m_comment;	//ZZZZZZZZ.
		MoveResult m_mr;	//ZZZZ'Z ZZZZZZ. 

		ArrayList		m_alLabel; //ZZZ ZZZZZ ZZ ZZZZ ZZZZ. 
		ArrayList		m_alMark; //ZZZZ

		//ZZZ ZZZZZ ZZ ZZZZ ZZZZZZ ZZZZZZ ZZ ZZZZ ZZZZ
		//ZZ ZZZZZ ZZ ZZZ ZZZZ ZZZZZ (ZZ ZZZ ZZZZZZ ZZZZ ZZZZ ZZ ZZZZZZZ). 
		ArrayList		m_alDead;
		StoneColor	m_cDead;
		/**
		 * ZZZZZZZZZZZ.
		 */
		public GoMove(int x, int y, StoneColor color, int seq) 
		{
			m_pos = new Point(x,y);
			m_c = color;
			m_n = seq;
			m_mr = new MoveResult();
			m_alLabel = new ArrayList();
			m_alMark = new ArrayList();
		}

		public GoMove(String str, StoneColor color) 
		{
			char cx = str[0];
			char cy = str[1];
			m_pos = new Point(0,0);
			//ZZZ Z# ZZ ZZZ ZZZZZZZZZ - 
			m_pos.X = (int) ( (int)cx - (int)(char)'a');
			m_pos.Y = (int) ( (int)cy - (int)(char)'a');
			this.m_c = color;
			m_alLabel = new ArrayList();
			m_alMark = new ArrayList();
		}


		private static Point	StrToPoint(String str)
		{
			Point p = new Point(0,0);
			char cx = str[0];
			char cy = str[1];
			//ZZZ Z# ZZ ZZZ ZZZZZZZZZ - 
			p.X = (int) ( (int)cx - (int)(char)'a');
			p.Y = (int) ( (int)cy - (int)(char)'a');
			return p;
		}


        public StoneColor Color
        { 
            get { return m_c; } 
        }

        public String Comment 
        {
            get
            {
                if (m_comment == null)
                    return string.Empty;
                else
                    return m_comment;
            }
            set
            {
                m_comment = value; 
            }
        }

		public int Seq
        {
            get { return m_n; }
            set {	m_n = value;}
        }

        public Point Point
        {
           get  { return m_pos; }
        }

        public MoveResult Result
        {
            get { return m_mr; }
            set { m_mr = value; }
        }
		
		public ArrayList DeadGroup
        {
            get { return m_alDead;}
            set {m_alDead = value;}
        }

        public StoneColor DeadGroupColor
        {
            get { return m_cDead; }
            set { m_cDead = value; }
        }
		
		public void AddLabel(String str) {m_alLabel.Add(StrToPoint(str));}
		
		public void AddMark(String str) {	m_alMark.Add(StrToPoint(str));}

        public ArrayList Labels
        {
            get { return m_alLabel; }
        }

        public ArrayList Marks
        {
            get { return m_alMark; }
        }
	}
	

	/**
	 * ZZZZZZZZZZ - ZZZ ZZZZZZ ZZ ZZZ 4 ZZZZZZZZZZZ ZZZZZZZZZ ZZZZ Z ZZZZ ZZ ZZZZZZ.
	 * 
	 */
	public class MoveResult 
	{
		public StoneColor color; 
		// 4 ZZZZZZZZ ZZZZZZ ZZZZZ ZZ ZZZZZZZZ. 
		public Boolean bUpKilled;
		public Boolean bDownKilled;
		public Boolean bLeftKilled;
		public Boolean bRightKilled;
		public Boolean bSuicide;	//ZZ ZZZ ZZZZ Z ZZZZZZZ?
		public MoveResult() 
		{
			bUpKilled = false;
			bDownKilled = false;
			bLeftKilled = false;
			bRightKilled = false;
			bSuicide = false;
		}
	}

	/**
	 * ZZZZZ. 
	 * ZZZZZ ZZZ ZZZ ZZZZZZZZ ZZZZZ, ZZZZZ ZZZ ZZZZZ. 
	 */
	public struct Stone 
	{
		public StoneColor color; 
	}

	/**
	 * Z ZZZZZZZZZ ZZ Z ZZ ZZZZ.
	 * ZZZZZZZZZZ: ZZZ ZZZZ ZZZZ ZZZZZZ ZZZ ZZ 0. 
	 */
	public class GoVariation 
	{
		//int m_id;			//ZZZZZZZZZ ZZ. 
		//string m_name;	//ZZZZZZZZZ ZZZZ. (ZZZZ.5, ZZZ.9, "ZZZZZ ZZZZZZ", ZZZ).
		//ZZZZZZZZZZZZZ ZZZ;	//ZZZZZZZZZ ZZZZZZZZ ZZZZZ.	
		ArrayList m_moves; 
		int m_seq;			//ZZZZZZ ZZZ ZZZ ZZ ZZZZ ZZZZ. 
		int m_total;

		//ZZZZZZZZZZZ. 
		public GoVariation()
		{
			//m_id = id;
			m_moves = new ArrayList(10);
			m_seq = 0;
			m_total = 0;
		}

		public void AddAMove(GoMove move) 
		{
			move.Seq = m_total ++;
			m_seq++;
			m_moves.Add(move);
		}

		public void UpdateResult(GoMove move) 
		{
		}

		public GoMove DoNext()
		{
			if (m_seq < m_total) 
			{
				return (GoMove)m_moves[m_seq++];
			} 
			else 
				return null;
		}

		public GoMove DoPrevious()
		{
			if (m_seq > 0)
				return (GoMove)(m_moves[--m_seq]);
			else 
				return null;
		}

		/*
		 *  ZZZZ ZZZZZZ ZZZ ZZZZZZZZ ZZZZ, ZZ ZZZZZZ ZZZZZZ ZZ ZZZ ZZZZZZZZ.
		 */
		public GoMove PeekPrevious()
		{
			if (m_seq > 0)
				return (GoMove)(m_moves[m_seq-1]);
			else 
				return null;
		}

		public void Reset() {m_seq = 0;}
	}


	/**
	* ZZ: ZZZ ZZ ZZ Z ZZZZZZZZZ ZZZZZ ZZZZ ZZZ ZZZZZZZ ZZZZ. 
	* ZZZ: ZZZ ZZZZ ZZ Z ZZZZZZZZZ ZZZZZ ZZZZZZZ ZZZZ. 
	*/
	struct VarStartPoint
	{
		//int m_id; 
		//int m_seq;
	}

	struct GameInfo 
	{
		public string gameName;
		public string playerBlack;
		public string playerWhite;
		public string rankBlack;
		public string rankWhite;
		public string result;
		public string date;
		public string km;
		public string size;
		public string comment;
        public string handicap;
        public string gameEvent;
        public string location;
        public string time;             // ZZZZZ ZZZZZ ZZZZ ZZZZZ ZZ ZZZ ZZZZ. 
        public string unknown_ff;   //ZZZZZZZ ZZZZZZZZZZ. 
        public string unknown_gm;
        public string unknown_vw; 
	}

	public class KeyValuePair 
	{
		public string key; public ArrayList alV;

		private static string	RemoveBackSlash(string strIn)
		{
			string strOut; 
			int		iSlash;

			strOut = string.Copy(strIn);
			if (strOut.Length < 2)
				return strOut;
			for (iSlash = strOut.Length-2; iSlash>=0; iSlash--)
			{
				if (strOut[iSlash] == '\\')		// && ZZZZZZ[ZZZZZZ+1] == ']')
				{
					strOut = strOut.Remove(iSlash,1);
					if (iSlash>0)
						iSlash --;	//ZZZZ ZZ ZZZZ ZZZZZZZZZ ZZZZZ ZZ ZZZZZZ ZZZ ZZZZZ
				}
			}
			return strOut;
		}

		public KeyValuePair(string key, string value)
		{
			this.key = string.Copy(key);
			string strOneVal;
			int		iBegin, iEnd;
		
			//ZZZZ ZZ ZZZZZ ZZZZ ZZZZZ
			alV = new ArrayList(1);

			//ZZZZZZZ ZZZZ ZZZ ZZZZZZZ Z[ZZZZZZZ]
			if (key.Equals("C"))
			{
				strOneVal = RemoveBackSlash(string.Copy(value));
				//ZZZ ZZZ ZZ '\'
				alV.Add(strOneVal);
				return;
			}

			iBegin = value.IndexOf("[");
			if (iBegin == -1)	//ZZZZZZ ZZZZZ
			{
				alV.Add(value);
				return; 
			}
			
			iBegin = 0;
			while (iBegin < value.Length && iBegin>=0)
			{
				iEnd = value.IndexOf("]", iBegin);
				if (iEnd > 0)
					strOneVal = value.Substring(iBegin, iEnd-iBegin);
				else 
					strOneVal = value.Substring(iBegin);	//ZZZ ZZZZ ZZZZZ
				alV.Add(strOneVal);
				iBegin = value.IndexOf("[", iBegin+1);
				if (iBegin > 0)
					iBegin ++;	//ZZZ ZZ ZZZ ZZZZZ ZZ ZZZZ ZZZZZ
			}
		}
	}

	/**
	 * ZZZ ZZZZZZ ZZ Z ZZ ZZZZ.
	 * ZZZZ ZZ ZZZ ZZZZ ZZ ZZZ ZZZZ ZZZZ, ZZ ZZZZZ Z ZZZZZ ZZ ZZZZZZZZZZ. 
	 */

	public class GoTree 
	{
		GameInfo _gi;		//ZZZZZ ZZZ ZZZZ'Z ZZZZZZZ ZZZZ.
		ArrayList _vars;		//ZZZZZZZZZZ. 
		//int _currVarId;		//ZZ ZZ ZZZZZZZ ZZZZZZZZZ.
		//int _currVarNum;
		GoVariation _currVar;		//ZZZZZZZ ZZZZZZZZZZZ.
		string	_stGameComment;

		// ZZZZZZZZZZZ - ZZZZZZ ZZZ ZZZZZZ ZZZZZ ZZ ZZZZZ ZZZZZZ
		public GoTree(string str)
		{
			_vars = new ArrayList(10);
			//_currVarNum = 0;
			//_currVarId = 0; 
			_currVar = new GoVariation();
			_vars.Add(_currVar);
			ParseFile(str);
		}

		//	ZZZZZZZZZZZ - ZZZZZZ ZZ ZZZZZ ZZZZZZ
		public GoTree()
		{
			_vars = new ArrayList(10);
			//_currVarNum = 0;
			//_currVarId = 0; 
			_currVar = new GoVariation();
			_vars.Add(_currVar);
		}

		public	string Info
		{
            get
            {
                return _gi.comment == null? string.Empty : _gi.comment;
            }
		}

		public void AddMove(GoMove move) 
		{
			_currVar.AddAMove(move);
		}

		/**
		 * ZZZZZ ZZZ ZZZZZ ZZZZ ZZ Z ZZZZZZ. 
		 */
		Boolean ParseFile(String goStr) 
		{
			int iBeg, iEnd=0; 
			while (iEnd < goStr.Length) 
			{
				if (iEnd > 0)
					iBeg = iEnd;
				else 
					iBeg = goStr.IndexOf(";", iEnd);
				iEnd = goStr.IndexOf(";", iBeg+1);
				if (iEnd < 0) //ZZ ZZZZ ";"
					iEnd = goStr.LastIndexOf(")", goStr.Length);		//ZZZ ZZZZ ZZZZZZZ ZZZZZZ ZZ ZZZZZZZZ ZZ ")"
				if (iBeg >= 0 && iEnd > iBeg) 
				{
					string section = goStr.Substring(iBeg+1, iEnd-iBeg-1);
					ParseASection(section);
				} 
				else 
					break;
			}
			return true;
		}

        /// <ZZZZZZZ>
        /// ZZZZ ZZZ ZZZZZ ZZ ZZZ ZZ ZZZZZ ZZZZZZ,
        /// ZZZZZZZ ZZ'Z ZZZ "]" ZZZZ,  
        /// ZZ ZZZ ZZ ZZZZ "\]",  ZZ ZZZZ ZZZZZZZZ, ZZZ ZZZZZZ ZZZ ZZZ ZZZZ "]", ZZ ZZZ ZZ ZZZZZZ. 
        /// </ZZZZZZZ>
        /// <ZZZZZ ZZZZ="ZZZ"></ZZZZZ>
        /// <ZZZZZZZ></ZZZZZZZ>
        static int FindEndofValueStr(String sec)
        {
            int i = 0;
            //ZZ ZZZZZZ ZZ'ZZ ZZZZZZZZ ZZZZ ZZZZZZZ ZZZ ZZZZZ ZZZZZZ.
            while (i >= 0)
            {
                i = sec.IndexOf(']', i+1);
                if (i > 0 && sec[i - 1] != '\\')
                    return i;    //ZZZZZZ ZZZ ZZZZZ ZZ "]". 
            }

            //ZZ ZZ ZZZZ ZZ ZZZ ']', ZZZ'Z ZZZZ ZZZ ZZZ ZZZ ZZ ZZZZZZ. 
            return sec.Length - 1;		//ZZZZ ZZ ZZZ ZZZZZ ZZ ZZZ ZZZZ ZZZZ ZZ ZZZ ZZZZZZ
        }
        
        static int FindEndofValueStrOld(String sec)
		{
			int i = 0;
            //ZZ ZZZZZZ ZZ'ZZ ZZZZZZZZ ZZZZ ZZZZZZZ ZZZ ZZZZZ ZZZZZZ. 
			bool fOutside = false;
			
			for (i=0; i<sec.Length;i++)
			{
				if (sec[i] == ']')
				{
					if (i>1 && sec[i-1] != '\\') //ZZ ZZ
						fOutside = true;
				}
				else if (char.IsLetter(sec[i]) && fOutside && i>0)
					return i-1;
				else if (fOutside && sec[i] == '[')
					fOutside = false;
			}
			return sec.Length-1;		//ZZZZ ZZ ZZZ ZZZZZ ZZ ZZZ ZZZZ ZZZZ ZZ ZZZ ZZZZZZ
		}

        private static string PurgeCRLFSuffix(string inStr)
        {
            int iLast = inStr.Length - 1; //ZZZZZ ZZ ZZZ ZZ ZZZZZZ. 

            if (iLast <= 0)
                return inStr; 

            while ((inStr[iLast] == '\r' || inStr[iLast] == '\n' || inStr[iLast] == ' '))
            {
                iLast--; 
            }
            if ((iLast+1) != inStr.Length)
                return inStr.Substring(0, iLast+1);  //ZZZ 2ZZ ZZZZZZZZZ ZZ ZZZ ZZZZZZ
            else
                return inStr; 
        }
 

		/**
		 * ZZZZZ Z ZZZZZZZ ZZ ZZZ ZZZZZZ ZZZZZZ. 
		 * Z ZZZZZZZ ZZZ ZZZ ZZZZZZ "ZZ {ZZ}"
		 * Z ZZ (ZZZ ZZZZZ ZZZZ) ZZZ ZZZ ZZZZZZ "ZZZ ZZZZZ {ZZZZZ}"
		 * ZZZZ: Z ZZZ ZZZ ZZZZZZZZZ ZZZZ ZZZZZZZZ ZZZZZZ, Z.Z. ZZZZZZ, ZZZZZ:  Z[ZZ][ZZ]. 
		 * Z ZZZ ZZ ZZZZZZ 
		 * Z ZZZZZ ZZ Z ZZZZZZ ZZZZZZZZ ZZ [ ZZZ ].
		 * ZZZZ: ZZZZZZZZ ( Z[ZZZZZZZZ]) ZZZ ZZZZ ZZZ ']' ZZZZZZZZZ ZZZZZZ ZZZ ZZZZZZZZ, ZZZZZ ZZ ZZZZZZZ ZZ "\]"
		 * Z.Z.  Z[ZZZZZ ZZZZZ ZZ [4,Z\] ZZ ZZZZZ ZZZZZZ]
         * 
		 */
		Boolean ParseASection(String sec) 
		{
			int iKey = 0;
			int iValue = 0;
			int iLastValue = 0;
			KeyValuePair kv;
			ArrayList Section = new ArrayList(10);
			
			try 
			{
				iKey = sec.IndexOf("[");
				if (iKey < 0)
				{
					return false;
				}
                sec = PurgeCRLFSuffix(sec);
 
				iValue = FindEndofValueStr(sec); //ZZZ.ZZZZZZZ("]", ZZZZ);
				iLastValue = sec.LastIndexOf("]");
				if (iValue <= 0 || iLastValue <= 1)
				{
					return false;
				}
				sec = sec.Substring(0,iLastValue+1);
				while (iKey > 0 && iValue > iKey)//ZZ ZZZZ ZZZZZ ZZ ZZZZZZZ
				{
					string key = sec.Substring(0,iKey);
					int iNonLetter = 0;
					while (!char.IsLetter(key[iNonLetter]) && iNonLetter < key.Length)
						iNonLetter ++;
					key = key.Substring(iNonLetter);
					//ZZZZ ZZ ZZZZ ZZZ ZZZZ ZZZZZZ ZZZZZZZ ZZ Z [] ZZZZ
					//ZZZZZZ = ZZZZZZZZZZZZZZZZZ(ZZZ);
					string strValue = sec.Substring(iKey+1, iValue-iKey-1);
					//ZZZ ZZ ZZZZ Z ZZZ/ZZZZZ ZZZZ
					kv = new KeyValuePair(key, strValue);
					Section.Add(kv);
					if (iValue >= sec.Length)
						break;
					sec = sec.Substring(iValue+1);
					iKey = sec.IndexOf("[");
					if (iKey > 0)
					{
						iValue = FindEndofValueStr(sec); //ZZZ.ZZZZZZZ("]",ZZZZ);
					}
				}
			}
			catch
			{
                return false;
            }

			ProcessASection(Section);
			return true;
		}


        /** 
         * ZZZZZZZ Z ZZZ ZZZ ZZZ ZZZZZZZZZZZZZ ZZZZZ
         * ZZ ZZZZZ ZZ Z ZZZZ, ZZ ZZZZZZ ZZZZZZZZZZZ.
         * ZZZZZZZZZZ, ZZZ ZZZZZZZ ZZZ ZZZZ ZZZZ ZZZ ZZZZ ZZ ZZZZ. 
         * 
         * ZZZZ: ZZ/ZZ ZZZ ZZZZZZZZZ ZZZ ZZZZ ZZZZZZZZ ZZZZZ ZZZZZZ, ZZZ Z ZZZZZ'Z ZZZZ ZZZ ZZZZZZZ ZZZ 
         */
        Boolean ProcessASection(ArrayList arrKV) 
		{
			Boolean fMultipleMoves = false;   //ZZZZZZZ ZZZZ ZZZZZZZ ZZZ ZZZZZZZZ ZZZZZ. 
			GoMove gm = null; 
            
			string key, strValue;

			for (int i = 0;i<arrKV.Count;i++)
			{
				key = ((KeyValuePair)(arrKV[i])).key;
				for (int j=0; j<((KeyValuePair)(arrKV[i])).alV.Count; j++)
				{
					strValue = (string)(((KeyValuePair)(arrKV[i])).alV[j]);

                    if (key.Equals("B"))   //ZZZZZ ZZZZZ
                    {
                        Debug.Assert(gm == null);
                        gm = new GoMove(strValue, StoneColor.Black);
                    }
                    else if (key.Equals("W"))  //ZZZZZ ZZZZZ
                    {
                        Debug.Assert(gm == null);
                        gm = new GoMove(strValue, StoneColor.White);
                    }
                    else if (key.Equals("C"))  //ZZZZZZZ
                    {
                        //ZZZZZ.ZZZZZZ(Z>0);
                        if (gm != null)
                            gm.Comment = strValue;
                        else	//ZZZ ZZ ZZ ZZZ ZZZZ ZZZZZZZ 
                            _gi.comment += strValue;
                    }
                    else if (key.Equals("L"))  //ZZZZZ
                    {
                        if (gm != null)
                            gm.AddLabel(strValue);
                        else	//ZZZ ZZ ZZ ZZZ ZZZZ ZZZZZZZ 
                            _stGameComment += strValue;
                    }

                    else if (key.Equals("M"))  //ZZZZ
                    {
                        if (gm != null)
                            gm.AddMark(strValue);
                        else	//ZZZ ZZ ZZ ZZZ ZZZZ ZZZZZZZ 
                            _gi.comment += strValue;
                    }
                    else if (key.Equals("AW"))		//ZZZ ZZZZZ ZZZZZ
                    {
                        fMultipleMoves = true;
                        gm = new GoMove(strValue, StoneColor.White);
                    }
                    else if (key.Equals("AB"))		//ZZZ ZZZZZ ZZZZZ
                    {
                        fMultipleMoves = true;
                        gm = new GoMove(strValue, StoneColor.Black);
                    }
                    else if (key.Equals("HA"))
                        _gi.handicap = (strValue);
                    else if (key.Equals("BR"))
                        _gi.rankBlack = (strValue);
                    else if (key.Equals("PB"))
                        _gi.playerBlack = (strValue);
                    else if (key.Equals("PW"))
                        _gi.playerWhite = (strValue);
                    else if (key.Equals("WR"))
                        _gi.rankWhite = (strValue);
                    else if (key.Equals("DT"))
                        _gi.date = (strValue);
                    else if (key.Equals("KM"))
                        _gi.km = (strValue);
                    else if (key.Equals("RE"))
                        _gi.result = (strValue);
                    else if (key.Equals("SZ"))
                        _gi.size = (strValue);
                    else if (key.Equals("EV"))
                        _gi.gameEvent = (strValue);
                    else if (key.Equals("PC"))
                        _gi.location = (strValue);
                    else if (key.Equals("TM"))
                        _gi.time = (strValue);
                    else if (key.Equals("GN"))
                        _gi.gameName = strValue;

                    else if (key.Equals("FF"))
                        _gi.unknown_ff = (strValue);
                    else if (key.Equals("GM"))
                        _gi.unknown_gm = (strValue);
                    else if (key.Equals("VW"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("US"))
                        _gi.unknown_vw = (strValue);

                    else if (key.Equals("BS"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("WS"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("ID"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("KI"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("SO"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("TR"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("LB"))
                        _gi.unknown_vw = (strValue);
                    else if (key.Equals("RO"))
                        _gi.unknown_vw = (strValue);


                    //ZZZZ ZZZZZ
                    else
                        System.Diagnostics.Debug.Assert(false, "unhandle key: " + key + " "+ strValue);

                    //ZZZZZZZZZ ZZZ ZZZZ ZZZZZZ ZZZ ZZZZ ZZ ZZZZ ZZZZ (ZZ, ZZ) ZZZZ ZZZZZZZZ ZZZZZ. 
                    if (fMultipleMoves)
                    {
                        _currVar.AddAMove(gm);
                    }
                }
            }

            //ZZZ ZZZ ZZZZ ZZ ZZZZZZZ ZZZZZZZZZ. 
            if (!fMultipleMoves && gm != null)
            {
                _currVar.AddAMove(gm);
            }
			return true;
		} 

		public GoMove DoPrevious() 
		{
			return _currVar.DoPrevious();
		}

		public GoMove PeekPrevious() 
		{
			return _currVar.PeekPrevious();
		}

		public GoMove DoNext() 
		{
			return _currVar.DoNext();
		}

		public void UpdateResult(GoMove move) 
		{
			_currVar.UpdateResult(move);
		}
		
		public void Reset()
		{
			//_currVarNum = 0;
			//_currVarId = 0; 
			_currVar.Reset();
		}
		public static void RewindToStart()
		{

		}
	} //ZZZ ZZ ZZZZZZ
}
