using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhoneNumberGenor{
    public static string GeneratePhoneNumber(int digit) {
        string phoneNumber = "";
        for (int i = 0; i < digit; i++) {
            phoneNumber += Random.Range(1, 10);
        }
        return phoneNumber;
    }
}
