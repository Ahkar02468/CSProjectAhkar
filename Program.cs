using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSProjectAhkar
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;
            while (year == 0)
            {
                Console.WriteLine("\nPlease enter the year.");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message + "Please try again.");
                }
            }
            while (month == 0)
            {
                Console.WriteLine("Please enter the month.");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Month must be between 1 and 12.");
                        month = 0;
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message + "Please try again month.");
                }
            }
            myStaff = fr.ReadFile();
            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("Enter hours worked for {0}",myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month,year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);
            Console.Read();
        }
        class Staff
        {
            private float hourlyRate;
            private int hWorked;
            public float TotalPay { get; protected set; }
            public float BasicPay { get; private set; }
            public string NameOfStaff { get; private set; }
            public int HoursWorked
            {
                get
                {
                    return hWorked;
                }
                set
                {
                    if (value > 0)
                        hWorked = value;
                    else
                        hWorked = 0;
                }
            }
            public Staff(string name,float rate)
            {
                hourlyRate = rate;
                NameOfStaff = name;
            }
            public virtual void CalculatePay()
            {
                Console.WriteLine("Calculating Pay...");
                BasicPay = hourlyRate * hWorked;
                TotalPay = BasicPay;
            }
            public override string ToString()
            {
                return "\nNameOfstaff = " + NameOfStaff
                    + "\nHourlyRate = " + hourlyRate + "\nWorked =" + hWorked
                    + "\nBasicPay = " + BasicPay + "\nTotalPay = " + TotalPay;

            }
        }
        class Manager : Staff
        {
            private const float managerHourlyRate = 50;
            public int Allowance { get; private set; }
            public Manager(string name) : base(name, managerHourlyRate) { }
            public override void CalculatePay()
            {
                base.CalculatePay();
                Allowance = 0;
                if(HoursWorked > 160)
                {
                    Allowance = 1000;
                    TotalPay = Allowance + BasicPay;
                }
            }
            public override string ToString()
            {
                return "\nNameOfStaff = " + NameOfStaff + "\nManagerHourlyRate = " + managerHourlyRate
                    + "\nAllowance = " + Allowance + "\nHourWorked = " + HoursWorked
                    + "\nBasicPay = " + BasicPay + "\nTotalPay = " + TotalPay;
            }
        }
        class Admin : Staff
        {
            private const float overtimeRate = 15.5f;
            private const float adminHourlyRate = 30f;
            public float Overtime { get; private set; }
            public Admin(string name) : base(name, adminHourlyRate) { }
            public override void CalculatePay()
            {
                base.CalculatePay();
                Overtime = 0;
                if (HoursWorked > 160)
                {
                    Overtime = overtimeRate * (HoursWorked - 160);
                    TotalPay = BasicPay + Overtime;
                }
            }
            public override string ToString()
            {
                return "\nNameOfStaff = " + NameOfStaff + "\nAdminHourlyRate = " + adminHourlyRate
                    + "\nHourWorked = " + HoursWorked + "\nBasicPay = " + BasicPay
                    + "\nOvertime = " + Overtime + "\nTotal = " + TotalPay;
            }

        }
        class FileReader
        {
            public List<Staff> ReadFile()
            {
                List<Staff> myStaff = new List<Staff>();
                string[] result = new string[2];
                string path = "staff.txt";
                string[] seperator = { ", " };
                if (File.Exists(path))
                {
                    using (StreamReader fileRead = new StreamReader(path))
                    {
                        while (!fileRead.EndOfStream)
                        {
                            result = fileRead.ReadLine().Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                            if (result[1] == "Manager")
                            {
                                myStaff.Add(new Manager(result[0]));
                            }
                            if (result[1] == "Admin")
                            {
                                myStaff.Add(new Admin(result[0]));
                            }
                        }
                        fileRead.Close();
                    }
                }
                else
                {
                    Console.WriteLine("The file doesn't exist.");
                }
                return myStaff;

            }
        }
        class PaySlip
        {
            private int month;
            private int year;
            enum MonthsOfYear
            {
                Jan=1,Feb=2,Mar,Apr, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC
            }
            public PaySlip(int payMonth,int payYear)
            {
                month = payMonth;
                year = payYear;
            }
            public void GeneratePaySlip(List<Staff> myStaff)
            {
                string path;
                foreach (Staff f in myStaff)
                {
                    path = f.NameOfStaff + ".txt";
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                        sw.WriteLine("=====================");
                        sw.WriteLine("Name Of Staff : {0}", f.NameOfStaff);
                        sw.WriteLine("Hours worked : {0}", f.HoursWorked);
                        sw.WriteLine("");
                        sw.WriteLine("Basic Pay : {0}", f.BasicPay);
                        sw.WriteLine("");
                        if (f.GetType() == typeof(Manager))
                        {
                            sw.WriteLine("Allowance : {0:C}", ((Manager)f).Allowance);
                        }
                        else if(f.GetType() == typeof(Admin))
                        {
                            sw.WriteLine("Overtime : {0:C}", ((Admin)f).Overtime);
                        }
                        sw.WriteLine("");
                        sw.WriteLine("====================");
                        sw.WriteLine("Total Pay: {0:C}", f.TotalPay);
                        sw.Close();
                    }
                }
            }
            public void GenerateSummary(List<Staff> myStaff)
            {
                var result
                    = from staff in myStaff
                      where staff.HoursWorked < 10
                      orderby staff.NameOfStaff ascending
                      select new { staff.NameOfStaff, staff.HoursWorked };
                string path = "summary.txt";
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine("Staff with less than 10 working hours");
                    sw.WriteLine("");
                    foreach (var f in result)
                    {
                        sw.WriteLine("Name Of Staff: {0},Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);
                    }
                   
                }
            }
            public override string ToString()
            {
                return "month = " + month + "year = " + year;
            }
        }
    }
}
