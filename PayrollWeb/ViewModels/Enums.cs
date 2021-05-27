﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PayrollWeb.ViewModels
{
    
    public enum  MaritalStatus
    {
        [Display(Name="Married")]
        Married,
        [Display(Name = "Unmarried")]
        Unmarried
    }


    public enum GFStatus
    {
         [Display(Name = "YES")]
         Y,
         [Display(Name = "NO")]
         N
    }
    public enum PFStatus
    {
        [Display(Name = "YES")]
        Y,
        [Display(Name = "NO")]
        N
    }
    public enum IsConfirmedStatus
    {
        [Display(Name = "YES")]
        Y,
         [Display(Name = "NO")]
        N
    }

    public enum Genders
    {
        [Display(Name = "MALE")]
        Male,
        [Display(Name = "FEMALE")]
        Female
    }
    
    public enum ConfirmedEmployee
    {
        [Display(Name = "Please Selete")]
              SELECT,
        [Display(Name = "YES")]
        YES,
        [Display(Name = "NO")]
        NO
    }

    public enum IsFestival
    {
        [Display(Name = "Please Selete")]
              SELECT,
        [Display(Name = "YES")]
        YES,
        [Display(Name = "NO")]
        NO
    }

    public enum IsTaxable
    {
        [Display(Name = "Please Selete")]
              SELECT,
        [Display(Name = "YES")]
        YES,
        [Display(Name = "NO")]
        NO
    }

    public enum IsHold
    {
        [Display(Name = "Please Selete")]
        SELECT,
        [Display(Name = "YES")]
        Hold,
        [Display(Name = "NO")]
        UnHold
    }

    public enum IsPayWithSalary
    {
        [Display(Name = "NO")]
        NO,
        [Display(Name = "YES")]
        YES
    }

    public enum IsProjected
    {
        [Display(Name = "NO")]
        NO,
        [Display(Name = "YES")]
        YES
    }

    public enum IsActive
    {
        [Display(Name = "NO")]
        NO,
        [Display(Name = "YES")]
        YES
    }

    public enum LeaveType
    {
        [Display(Name = "Please Select")]
        SELECT,
        [Display(Name = "Basic")]
        Basic
    }
}