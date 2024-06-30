using System.Collections.Generic;
using System.Linq;

public static class Utils {

    public static string Clockify(int number) => (number <= 9 ? "0" : "") + number.ToString();
    public static string TimerText(int hours, int minutes, int seconds) =>
		// Include hours if they're there 
		(hours == 0 ? "" : $"{Clockify(hours)}:") +
		// always show minutes and seconds 
		$"{Clockify(minutes)}:{Clockify(seconds)} ";
}