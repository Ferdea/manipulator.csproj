using System;
using System.Drawing;
using NUnit.Framework;
using MyExtends;

namespace Manipulation
{
    public static class AnglesToCoordinatesTask
    {
        /// <summary>
        /// По значению углов суставов возвращает массив координат суставов
        /// в порядке new []{elbow, wrist, palmEnd}
        /// </summary>
        public static PointF[] GetJointPositions(double shoulder, double elbow, double wrist)
        {
            var elbowPos = new PointF(
                (float)(Manipulator.UpperArm * Math.Cos(shoulder)), 
                (float)(Manipulator.UpperArm * Math.Sin(shoulder))
                );
            var wristPos = 
                new PointF(
                    (float)(Manipulator.Forearm * Math.Cos(elbow + shoulder - Math.PI)), 
                    (float)(Manipulator.Forearm * Math.Sin(elbow + shoulder - Math.PI))
                    )
                + new SizeF(elbowPos.X, elbowPos.Y);
            var palmEndPos = 
                new PointF(
                    (float)(Manipulator.Palm * Math.Cos(wrist + elbow + shoulder - 2 * Math.PI)), 
                    (float)(Manipulator.Palm * Math.Sin(wrist + elbow + shoulder - 2 * Math.PI))
                    )
                + new SizeF(wristPos.X, wristPos.Y);
            return new PointF[]
            {
                elbowPos,
                wristPos,
                palmEndPos
            };
        }
    }

    [TestFixture]
    public class AnglesToCoordinatesTask_Tests
    {
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI, 
            Manipulator.Forearm + Manipulator.Palm, Manipulator.UpperArm)]
        [TestCase(0.0, Math.PI, Math.PI, 
            Manipulator.Forearm + Manipulator.UpperArm + Manipulator.Palm, 0.0f)]
        [TestCase(0.0, 0.0, 0.0, 
            -Manipulator.Forearm + Manipulator.UpperArm + Manipulator.Palm, 0.0f)]
        [TestCase(Math.PI / 2, Math.PI, Math.PI,
            0.0f, Manipulator.Forearm + Manipulator.UpperArm + Manipulator.Palm)]
        [TestCase(-Math.PI / 2, 0.0, Math.PI, 
            0.0f, Manipulator.Forearm - Manipulator.UpperArm + Manipulator.Palm)]
        [TestCase(Math.PI / 2, Math.PI / 2, Math.PI / 2,
            Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm)]
        [TestCase(Math.PI / 2, 3 * Math.PI / 2, 3 * Math.PI / 2, 
            - Manipulator.Forearm, Manipulator.UpperArm - Manipulator.Palm)]
        [TestCase(Math.PI / 2, Math.PI, 3 * Math.PI, 
            0.0f, Manipulator.Forearm + Manipulator.UpperArm + Manipulator.Palm)]
        public void TestGetJointPositions(double shoulder, double elbow, double wrist, double palmEndX, double palmEndY)
        {
            var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
            Assert.AreEqual(palmEndX, joints[2].X, 1e-5, "palm endX");
            Assert.AreEqual(palmEndY, joints[2].Y, 1e-5, "palm endY");
            Assert.AreEqual(Manipulator.UpperArm, joints[0].GetLength());
            Assert.AreEqual(Manipulator.Forearm, (joints[1] - new SizeF(joints[0])).GetLength());
            Assert.AreEqual(Manipulator.Palm, (joints[2] - new SizeF(joints[1])).GetLength());
        }
    }
}

namespace MyExtends
{
    public static class MyExtendMethods
    {
        /// <returns>Возвращает длину радиус-вектора точки p</returns>
        public static float GetLength(this PointF p) => (float)Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));
    }

    [TestFixture]
    public class MyExtendMethods_Tests
    {
        [Test]
        public static void GetLength_IsZero_WhenPIsZeroVector()
        {
            var expected = 0.0f;
            var actual = new PointF(0.0f, 0.0f).GetLength();
            Assert.AreEqual(expected, actual, 1e-5);
        }
        
        [Test]
        public static void GetLength_WhenXIsZero()
        {
            var expected = 17.5f;
            var actual = new PointF(0.0f, 17.5f).GetLength();
            Assert.AreEqual(expected, actual, 1e-5);
        }
        
        [Test]
        public static void GetLength_WhenYIsZero()
        {
            var expected = 21.66666f;
            var actual = new PointF(21.66666f, 0.0f).GetLength();
            Assert.AreEqual(expected, actual, 1e-5);
        }
        
        [Test]
        public static void GetLength_Test()
        {
            var expected = Math.Sqrt(2);
            var actual = new PointF(1.0f, 1.0f).GetLength();
            Assert.AreEqual(expected, actual, 1e-5);
        }
    }
}