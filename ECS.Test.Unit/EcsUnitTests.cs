using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

// Jenkins test instance: http://ci3.ase.au.dk:8080/job/ECS_Gruppe7/

namespace ECS.Test.Unit
{
    [TestFixture]
    public class ECSUnitTest
    {
        private ECS _uut;
        private IHeater _heater;
        private ITempSensor _sensor;
        private IWindow _window;
        private int _lowerTemperatureThreshold = -50;
        private int _upperTemperatureThreshold = 50;

        [SetUp]
        public void Setup()
        {
            _heater = Substitute.For<IHeater>();
            _sensor = Substitute.For<ITempSensor>();
            _window = Substitute.For<IWindow>();

            _uut = new ECS(_sensor, _heater, _window, _lowerTemperatureThreshold, _upperTemperatureThreshold);
        }

       
        [Test]
        public void GetTemp_TempLowerThanThreshold()
        {
            _sensor.GetTemp().Returns(-51);
            _uut.Regulate();

            _heater.Received().TurnOn();
            _window.Received().Close();
        }

        [TestCase(10, 20, 5)]
        [TestCase(6, 10, 5)]
        [TestCase(0, 100, -10)]
        public void GetTemp_TempLowerThanThreshold_TestCasesExample(int lowerThreshold, int upperThreshold, int temp)
        {
            _uut.UpperTemperatureThreshold = upperThreshold;
            _uut.LowerTemperatureThreshold = lowerThreshold;
            _sensor.GetTemp().Returns(temp);
            _uut.Regulate();

            _heater.Received().TurnOn();
            _window.Received().Close();
        }

        [TestCase(1, 0)]
        [TestCase(24, 19)]
        public void LowerThreshold_GreaterThan_UpperThreshold(int lowerThreshold, int upperThreshold)
        {
            //_uut.LowerTemperatureThreshold = 0;

            _uut.UpperTemperatureThreshold = upperThreshold;
            Assert.Throws<ArgumentException>( () => _uut.LowerTemperatureThreshold = lowerThreshold);
        }

        [TestCase(19, 15)]
        [TestCase(1, 0)]
        public void UpperThreshold_LowerThan_LowerThreshold(int lowerThreshold, int upperThreshold)
        {

            //_uut.UpperTemperatureThreshold = lowerThreshold + 1;

            _uut.LowerTemperatureThreshold = lowerThreshold;
            Assert.Throws<ArgumentException>(() => _uut.UpperTemperatureThreshold = upperThreshold);
        }

    }
}
