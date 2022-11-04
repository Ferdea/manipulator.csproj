using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Manipulation
{
	public static class VisualizerTask
	{
		public static double X = 220;
		public static double Y = -100;
		public static double Alpha = 0.05;
		public static double Wrist = 2 * Math.PI / 3;
		public static double Elbow = 3 * Math.PI / 4;
		public static double Shoulder = Math.PI / 2;

		public static Brush UnreachableAreaBrush = new SolidBrush(Color.FromArgb(255, 255, 230, 230));
		public static Brush ReachableAreaBrush = new SolidBrush(Color.FromArgb(255, 230, 255, 230));
		public static Pen ManipulatorPen = new Pen(Color.Black, 3);
		public static Brush JointBrush = Brushes.Gray;

		public static void KeyDown(Form form, KeyEventArgs key)
		{
			var angleRotation = Math.PI / 250.0;
			switch (key.KeyCode)
			{
				case Keys.Q:
					Shoulder += angleRotation;
					break;
				case Keys.A:
					Shoulder -= angleRotation;
					break;
				case Keys.W:
					Elbow += angleRotation;
					break;
				case Keys.S:
					Elbow -= angleRotation;
					break;
			}

			Wrist = -(Alpha + Shoulder + Elbow);
			form.Invalidate();
		}


		public static void MouseMove(Form form, MouseEventArgs e)
		{
			var newCoords = ConvertWindowToMath(e.Location, GetShoulderPos(form));
			X = newCoords.X;
			Y = newCoords.Y;
			UpdateManipulator();
			form.Invalidate();
		}

		private static float sign(float f) => f > 0 ? 1.0f : (f < 0 ? -1 : 0);

		public static void MouseWheel(Form form, MouseEventArgs e)
		{
			Alpha += Math.PI * sign(e.Delta) / 250.0f;
			UpdateManipulator();
			form.Invalidate();
		}

		public static void UpdateManipulator()
		{
			var angles = ManipulatorTask.MoveManipulatorTo(X, Y, Alpha);
            if (double.IsNaN(angles[0]) && double.IsNaN(angles[1]) && double.IsNaN(angles[2]))
	            return;
            Shoulder = angles[0];
            Elbow = angles[1];
            Wrist = angles[2];
		}

		public static void DrawManipulator(Graphics graphics, PointF shoulderPos)
		{
			var joints = new[] {
					new[] { new PointF(0.0f, 0.0f) },
					AnglesToCoordinatesTask.GetJointPositions(Shoulder, Elbow, Wrist) }
				.SelectMany(g => g).ToArray();

			graphics.DrawString(
                $"X={X:0}, Y={Y:0}, Alpha={Alpha:0.00}", 
                new Font(SystemFonts.DefaultFont.FontFamily, 12), 
                Brushes.DarkRed, 
                10, 
                10);
			DrawReachableZone(graphics, ReachableAreaBrush, UnreachableAreaBrush, shoulderPos, joints.Skip(1).ToArray());
			
			for (var i = 1; i <= 3; i++)
				graphics.DrawLine(ManipulatorPen,
				ConvertMathToWindow(joints[i - 1], shoulderPos), 
				ConvertMathToWindow(joints[i], shoulderPos));
			for (var i = 1; i <= 3; i++)
				graphics.FillEllipse(JointBrush,
					new RectangleF(ConvertMathToWindow(joints[i], shoulderPos) - new SizeF(5.0f, 5.0f),
						new SizeF(10.0f, 10.0f)));
		}

		private static void DrawReachableZone(
            Graphics graphics, 
            Brush reachableBrush, 
            Brush unreachableBrush, 
            PointF shoulderPos, 
            PointF[] joints)
		{
			var rmin = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
			var rmax = Manipulator.UpperArm + Manipulator.Forearm;
			var mathCenter = new PointF(joints[2].X - joints[1].X, joints[2].Y - joints[1].Y);
			var windowCenter = ConvertMathToWindow(mathCenter, shoulderPos);
			graphics.FillEllipse(reachableBrush, windowCenter.X - rmax, windowCenter.Y - rmax, 2 * rmax, 2 * rmax);
			graphics.FillEllipse(unreachableBrush, windowCenter.X - rmin, windowCenter.Y - rmin, 2 * rmin, 2 * rmin);
		}

		public static PointF GetShoulderPos(Form form)
		{
			return new PointF(form.ClientSize.Width / 2f, form.ClientSize.Height / 2f);
		}

		public static PointF ConvertMathToWindow(PointF mathPoint, PointF shoulderPos)
		{
			return new PointF(mathPoint.X + shoulderPos.X, shoulderPos.Y - mathPoint.Y);
		}

		public static PointF ConvertWindowToMath(PointF windowPoint, PointF shoulderPos)
		{
			return new PointF(windowPoint.X - shoulderPos.X, shoulderPos.Y - windowPoint.Y);
		}
	}
}