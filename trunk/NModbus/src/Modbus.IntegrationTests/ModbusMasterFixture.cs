using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Modbus.Device;
using System.IO.Ports;

namespace Modbus.IntegrationTests
{
	// NOTE: this integration test requires a Modbus Slave running at the configuration below
	[TestFixture]
	public class ModbusMasterFixture
	{
		private SerialPort _port;
		private const string PortName = "COM4";
		private const byte SlaveAddress = 2;

		[SetUp]
		public void SetUp()
		{
			_port = new SerialPort(PortName);
			_port.ReadTimeout = Modbus.DefaultTimeout;
			_port.Parity = Parity.None;
			_port.Open();
		}

		[TearDown]
		public void TearDown()
		{
			_port.Close();
			_port.Dispose();
		}

		[Test]
		public void ReadCoils()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);
			bool[] coils = master.ReadCoils(SlaveAddress, 100, 1);
			Assert.AreEqual(new bool[] { true }, coils);
		}

		[Test]
		public void ReadInputs()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);

			bool[] inputs = master.ReadInputs(SlaveAddress, 150, 3);
			Assert.AreEqual(new bool[] { true, true, true }, inputs);
		}

		[Test]
		public void ReadHoldingRegisters()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);
			ushort[] registers = master.ReadHoldingRegisters(SlaveAddress, 104, 2);
			Assert.AreEqual(new ushort[] { 104, 105 }, registers);
		}

		[Test]
		public void WriteSingleCoil()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);

			bool coilValue = master.ReadCoils(SlaveAddress, 105, 1)[0];
			master.WriteSingleCoil(SlaveAddress, 105, !coilValue);
			Assert.AreEqual(!coilValue, master.ReadCoils(SlaveAddress, 105, 1)[0]);
			master.WriteSingleCoil(SlaveAddress, 105, coilValue);
			Assert.AreEqual(coilValue, master.ReadCoils(SlaveAddress, 105, 1)[0]);
		}

		[Test]
		public void WriteSingleRegister()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);
			ushort testAddress = 200;
			ushort testValue = 350;

			ushort originalValue = master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0];
			master.WriteSingleRegister(SlaveAddress, testAddress, testValue);
			Assert.AreEqual(testValue, master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
			master.WriteSingleRegister(SlaveAddress, testAddress, originalValue);
			Assert.AreEqual(originalValue, master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
		}

		[Test]
		public void WriteMultipleRegisters()
		{
			ModbusASCIIMaster master = new ModbusASCIIMaster(_port);
			ushort testAddress = 200;
			ushort[] testValues = new ushort[] { 10, 20, 30, 40, 50 };
		
			ushort[] originalValues = master.ReadHoldingRegisters(SlaveAddress, testAddress, 5);
			master.WriteMultipleRegisters(SlaveAddress, testAddress, (ushort) testValues.Length, testValues);
			Assert.AreEqual(testValues, master.ReadHoldingRegisters(SlaveAddress, testAddress, 5));
			master.WriteMultipleRegisters(SlaveAddress, testAddress, (ushort) originalValues.Length, originalValues);
		}
	}
}
