using System;
using System.Drawing;
using NUnit.Framework;
using MyExtends;

namespace Manipulation
{
    public static class ManipulatorTask
    {
        /// <summary>
        /// Возвращает массив углов (shoulder, elbow, wrist),
        /// необходимых для приведения эффектора манипулятора в точку x и y 
        /// с углом между последним суставом и горизонталью, равному alpha (в радианах)
        /// См. чертеж manipulator.png!
        /// </summary>
        public static double[] MoveManipulatorTo(double x, double y, double alpha)
        {
            if (new PointF((float)x, (float)y).GetLength() > Manipulator.UpperArm + Manipulator.Forearm + Manipulator.Palm)
                return new[] { double.NaN, double.NaN, double.NaN };
            var wristPos = new PointF(
                (float)(-Manipulator.Palm * Math.Cos(alpha) + x),
                (float)(Manipulator.Palm * Math.Sin(alpha) + y)
                );
            var elbow = TriangleTask.GetABAngle(Manipulator.UpperArm, Manipulator.Forearm, wristPos.GetLength());
            var shoulder = Math.Atan2(wristPos.Y, wristPos.X)
                + TriangleTask.GetABAngle(Manipulator.UpperArm, wristPos.GetLength(), Manipulator.Forearm);
            var wrist = -(alpha + elbow + shoulder);
            return new[] { shoulder, elbow, wrist };
        }
    }

    [TestFixture]
    public class ManipulatorTask_Tests
    {
        [Test]
        public void TestMoveManipulatorTo()
        {
            var random = new Random();
            var x = random.Next(-1000, 1000);
            var y = random.Next(-1000, 1000);
            var alpha = random.NextDouble() * 2 * Math.PI;
            var angles = ManipulatorTask.MoveManipulatorTo(x, y, alpha);
            var actual = AnglesToCoordinatesTask.GetJointPositions(angles[0], angles[1], angles[2]);
            if (new PointF(x, y).GetLength() > Manipulator.UpperArm + Manipulator.Forearm + Manipulator.Palm)
            {
                Assert.IsNaN(actual[2].X);
                Assert.IsNaN(actual[2].Y);
            }
            else
            {
                Assert.AreEqual(x, actual[2].X, 1e-9, "x");
                Assert.AreEqual(y, actual[2].Y, 1e-9, "y");
            }
        }
    }
}