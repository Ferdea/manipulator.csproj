using System;
using NUnit.Framework;

namespace Manipulation
{
    public class TriangleTask
    {
        /// <summary>
        /// Возвращает угол (в радианах) между сторонами a и b в треугольнике со сторонами a, b, c 
        /// </summary>
        public static double GetABAngle(double a, double b, double c)
        {
            if (a <= 0.0 || b <= 0.0 || c < 0)
                return double.NaN;
            var angleCos = (a * a + b * b - c * c) / (2 * a * b);
            return Math.Acos(angleCos);
        }
    }

    [TestFixture]
    public class TriangleTask_Tests
    {
        [TestCase(3, 4, 5, Math.PI / 2)]
        [TestCase(1, 1, 1, Math.PI / 3)]
        [TestCase(1, 0, 1, double.NaN)]
        [TestCase(0, 1, 1, double.NaN)]
        [TestCase(1, 1, 0, 0)]
        [TestCase(-1, 1, 1, double.NaN)]
        [TestCase(1, -1, 1, double.NaN)]
        [TestCase(1, 1, -1, double.NaN)]
        public void TestGetABAngle(double a, double b, double c, double expectedAngle)
        {
            var actual = TriangleTask.GetABAngle(a, b, c);
            Assert.AreEqual(expectedAngle, actual, 1e-5);
        }
    }
}