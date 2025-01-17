﻿//===================================================
// Copyright (c)  coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Pease
//===================================================

namespace HR.DataAccess.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public decimal Salary { get; set; }
        public Address Address { get; set; }
    }
}
