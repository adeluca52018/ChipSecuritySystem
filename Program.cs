using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    public class Program
    {
        private const Color BeginningMarker = Color.Blue;
        private const Color EndingMarker = Color.Green;
        private static List<List<ColorChip>> overallList = new List<List<ColorChip>>();

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter 1 if you would like to make the bag of chips, otherwise a default bag will be provided");
            string userSelection = Console.ReadLine();

            List<ColorChip> bagOfChips = new List<ColorChip>();

            if (userSelection != "1")
            {
                bagOfChips = DefaultBagOfChipsBuilder();
            }
            else
            {
                bagOfChips = UserSuppliesChips();
            }

            List<ColorChip> initalBagOfChips = new List<ColorChip>(bagOfChips);

            var StartLinkResult = StartLink(bagOfChips, overallList);

            BuildLink(StartLinkResult.Item1, bagOfChips, StartLinkResult.Item2);
            var NarrorChipsResult = RemoveUnusedChips(overallList);
            DisplayResults(NarrorChipsResult, bagOfChips, initalBagOfChips);

            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        private static List<ColorChip> DefaultBagOfChipsBuilder()
        {
            List<ColorChip> chips = new List<ColorChip>();

            ColorChip chip1 = new ColorChip(Color.Blue, Color.Yellow);
            ColorChip chip2 = new ColorChip(Color.Red, Color.Green);
            ColorChip chip3 = new ColorChip(Color.Yellow, Color.Red);
            ColorChip chip4 = new ColorChip(Color.Orange, Color.Purple);

            chips.Add(chip1);
            chips.Add(chip2);
            chips.Add(chip3);
            chips.Add(chip4);

            return chips;
        }

        private static List<ColorChip> UserSuppliesChips()
        {

            Console.WriteLine("Please enter your chips one at a time in the following format: 'Blue,Green' 'Red,Purple' where start color and end color are seperated from a comma. You may only use the follownig colors");
            Console.WriteLine();

            Type type = typeof(Color);
            Array arrColors = type.GetEnumValues();

            foreach (var color in arrColors)
            {
                Console.WriteLine(color.ToString());
            }

            Console.WriteLine();

            List<ColorChip> lstChips = GetUserInputForChip();

            return lstChips;
        }

        private static List<ColorChip> GetUserInputForChip()
        {
            List<ColorChip> lstChips = new List<ColorChip>();

            try
            {
                bool done = false;

                while (!done) 
                {
                    Console.WriteLine("Please enter a chip. If done, type 'done'");
                    string input = Console.ReadLine();

                    if (input == "done")
                    {
                        done = true;
                        return lstChips;
                    }

                    string[] colors = input.Split(',');

                    colors[0] = colors[0].Substring(0, 1).ToUpper() + colors[0].Substring(1);
                    colors[1] = colors[1].Substring(0, 1).ToUpper() + colors[1].Substring(1);

                    Color StartColor = (Color)Enum.Parse(typeof(Color), colors[0]);
                    Color EndColor = (Color)Enum.Parse(typeof(Color), colors[1]);

                    ColorChip chip = new ColorChip(StartColor, EndColor);
                    lstChips.Add(chip);
                }
                return lstChips;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The chip input was invalid. The bag is now closed");
                return lstChips;

            }
        }

        public static Tuple<List<ColorChip>, List<List<ColorChip>>> StartLink(List<ColorChip> bagOfChips, List<List<ColorChip>> overallList)
        {
            try
            {
                bool hasStartingChip = bagOfChips.Exists(x => x.StartColor == BeginningMarker);
                bool hasEndingChip = bagOfChips.Exists(x => x.EndColor == EndingMarker);

                if (!hasStartingChip || !hasEndingChip)
                {
                    throw new Exception(Constants.ErrorMessage);
                }

                Color CursorColor = BeginningMarker;

                List<ColorChip> lstChips = bagOfChips.FindAll(x => x.StartColor == CursorColor);

                if (lstChips.Count() > 0 && lstChips != null)
                {
                    overallList.Add(lstChips);

                    return new Tuple<List<ColorChip>, List<List<ColorChip>>>(lstChips, overallList);
                }
                else
                {
                    throw new Exception(Constants.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                DisplayErrorAndExit(ex.Message);
                return null;
            }

        }

        public static void BuildLink(List<ColorChip> lstChips, List<ColorChip> bagOfChips, List<List<ColorChip>> overallList)
        {
            try
            {
                Color cursorColor = lstChips[0].EndColor;
                List<ColorChip> lstSubchips = new List<ColorChip>();

                foreach (var chip in lstChips)
                {
                    cursorColor = chip.EndColor;

                    List<ColorChip> lstResult = bagOfChips.FindAll(x => x.StartColor == cursorColor);

                    bagOfChips.Remove(chip);

                    lstSubchips.AddRange(lstResult);
                }

                if (lstSubchips.Count > 0 && lstSubchips != null)
                {
                    lstSubchips = lstSubchips.Distinct().ToList();
                    overallList.Add(lstSubchips);
                    BuildLink(lstSubchips, bagOfChips, overallList);
                }
            }
            catch (Exception ex)
            {
                DisplayErrorAndExit(ex.Message);
            }

        }

        public static List<List<ColorChip>> RemoveUnusedChips(List<List<ColorChip>> overallList)
        {
            try
            {
                for (int i = overallList.Count() - 1; i >= 0; i--)
                {
                    if (i == overallList.Count() - 1)
                    {
                        overallList[i].RemoveAll(x => x.EndColor != EndingMarker);

                        if (!overallList[i].Exists(x => x.EndColor == EndingMarker))
                        {
                            throw new Exception(Constants.ErrorMessage);
                        }
                    }
                    else
                    {

                        List<ColorChip> nextLst = overallList[i + 1];

                        if (nextLst.Count() == 0)
                        {
                            overallList.Remove(nextLst);
                            continue;
                        }

                        overallList[i].RemoveAll(x => !nextLst.Select(y => y.StartColor).Contains(x.EndColor));
                    }
                }
                return overallList;
            }
            catch (Exception ex)
            {
                //error handling
                DisplayErrorAndExit(ex.Message);
                return null;
            }


        }

        public static void DisplayResults(List<List<ColorChip>> overallList, List<ColorChip> bagOfChips, List<ColorChip> bagOfChipsInitial)
        {
            try
            {
                int totalChipsUsed = overallList.Count();
                List<ColorChip> leftOverChips = new List<ColorChip>(bagOfChipsInitial);

                StringBuilder displayResults = new StringBuilder();
                displayResults.AppendLine("Link Successfully completed!");
                displayResults.AppendLine();

                displayResults.AppendLine("Starting Chips");
                displayResults.AppendLine();

                foreach (ColorChip chip in bagOfChipsInitial)
                {
                    displayResults.AppendLine("[" + chip + "]");
                }

                displayResults.AppendLine();

                displayResults.AppendLine("Total Chips Used: " + totalChipsUsed);
                displayResults.AppendLine();

                displayResults.AppendLine("The completed Link");
                displayResults.AppendLine();

                displayResults.Append(BeginningMarker + "|");

                foreach (List<ColorChip> chip in overallList)
                {
                    leftOverChips.Remove(chip[0]);
                    displayResults.Append("[" + chip[0] + "]");
                }

                displayResults.AppendLine("|" + EndingMarker);

                displayResults.AppendLine();
                displayResults.AppendLine("Left over chips");
                displayResults.AppendLine();


                foreach (ColorChip chip in leftOverChips)
                {
                    displayResults.AppendLine("[" + chip + "]");
                }

                Console.WriteLine(displayResults);

                Console.WriteLine();
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                DisplayErrorAndExit(ex.Message);
            }

        }

        private static void DisplayErrorAndExit(string errorMessage)
        {
            Console.WriteLine(errorMessage);
            Console.WriteLine();


            Console.WriteLine("Press any key to close");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
