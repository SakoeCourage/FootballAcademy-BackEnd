namespace FootballAcademy_BackEnd.Utilities
{
    public class Stringutilities
    {

        public static string GenerateRandomOtp()
        {
            Random random = new Random();
            // Generate a random 6-digit number
            int otpNumber = random.Next(100000, 999999);
            // Convert the number to a string with leading zeros if necessary
            string otp = otpNumber.ToString("D6");

            return otp;
        }
    }
}
