/* Name: Muhd Hizam Bin Kamis
 * Matric No.: U095858L
 * 
 * Personal Budget Assistant
 * 
 * ---------------------------------------------------------------------------------
 * Description:
 * This program takes in a list of items in the following format (console input):
 *
 * [number of Items]
 * [item name] [price] [satisfaction value] // list of items
 * [budget]
 *
 * It then selects a subset of the items that maximises the total satisfaction
 * value within the budget.
 * ---------------------------------------------------------------------------------
 * Algorithm:
 * This program makes use of the knapsack problem algorithm. It creates two
 * tables (i.e. Satisfaction and Keep truth table) whereby the items are analysed
 * sequentially and the satisfaction and boolean keep values are updated into the 
 * tables.
 * 
 * After both tables are completed, the satisfaction table is analysed iteratively to 
 * determine the maximum satisfaction value obtained. If the maximum satisfaction value
 * is encountered, the list of items that give rise to this value will be determined from 
 * the Keep truth table and compiled in a list. This list is then added to another main 
 * list for output.
 * 
 * The algorithm is not very time-efficient (O(n^2)), but the tables can be used multiple 
 * times once they have been generated. 
 * ---------------------------------------------------------------------------------
 * Considerations:
 * The following 2 special cases are to be considered for the output
 * 1) Different sublist of items give rise to the same maximum satisfaction value and same
 * total cost price.
 * 2) Different sublist of items give rise to the same maximum satisfaction value but 
 * different total cost price.
 * 
 * For both of these cases, the program will output the list of different combinations first. 
 * It will then output the best combination (i.e. lowest total price & lowest no. of items).
 * ---------------------------------------------------------------------------------
 * */
// Hello! How are you!


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

// Main Class
class Program
{
    static void Main(string[] args)
    {

        try {
        int n, b, SValue = 0, BValue = 0;
        double ItemsBudget;
        String ItemString, NumberOfItems, ItemBudget;

        // StreamReader sr = new StreamReader("Testcase 2.txt");

        // Input the number of items
        Console.WriteLine("Welcome to Personal Budget Assistant V0.1!");
        Console.WriteLine("");
        Console.WriteLine("Input the number of items: ");
        NumberOfItems = Console.ReadLine();
        bool check = int.TryParse(NumberOfItems, out n);

        if (n < 1 || check == false)
        {
            check = false;
            while (!check)
            {   
                Console.WriteLine("Please input a valid positive number");
                NumberOfItems = Console.ReadLine();
                check = int.TryParse(NumberOfItems, out n);
                if (n < 1)
                {
                    check = false;
                }
            }
        }

        else
        {
            if (int.TryParse(NumberOfItems, out n))
            {
            }
        }

        // Creating list of items
        List<Item> items = new List<Item>();

        // Input details for the items
        // The price is read as double from user but is then converted 
        // to integer (multiply by 100 and convert to Int32) to create
        // the knapsack tables
        Console.WriteLine("");
        Console.WriteLine("Input the item details: ");
        for (int i = 1; i <= n; i++)
        {
            ItemString = Console.ReadLine();
            Item a = new Item(ItemString);
            items.Add(a);
        }

        //Input the budget
        Console.WriteLine("");
        Console.WriteLine("Input the budget: ");
        ItemBudget = Console.ReadLine();
        ItemsBudget = Convert.ToDouble(ItemBudget);
        b = Convert.ToInt32(ItemsBudget * 100);

        // Process the data
        KnapSackProblem problem = new KnapSackProblem();
        int totalValueOfItems = 0;
        List<Item> bestCombItems = problem.FindItemsToBuy(items, b, out totalValueOfItems);

        // Prints out the suggested list of items
        if (bestCombItems.Count() != 0)
        {
            Console.WriteLine("Best Combination: ");
            for (int j = 1; j <= bestCombItems.Count; j++)
            {
                Console.Write(j + ") ");
                bestCombItems[j - 1].printout();
                SValue = SValue + bestCombItems[j - 1].Satisfaction;
                BValue = BValue + bestCombItems[j - 1].Price;
            }

            // Prints out the Satisfaction Value
            Console.WriteLine("");
            Console.WriteLine("Satisfaction:");
            Console.WriteLine(SValue);

            // Prints out the Amount Spent
            Console.WriteLine("");
            decimal d = new decimal(BValue, 0, 0, false, 2);
            Console.WriteLine("Total spent: ");
            Console.WriteLine("$" + d);
 
        }

        }
        catch (IOException e)
        {
            Console.WriteLine("Error Message. Cannot Stream Text File.");
            Console.WriteLine("Please check that the file is in the correct folder or refer to the readme.txt.");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error Message.");
            Console.WriteLine("Please ensure that input format is correct and restart the program.");
            Console.WriteLine("You may also wish to refer to the readme.txt to check the input format.");
        }

        // Press Enter to end the program
        Console.WriteLine();
        Console.WriteLine("Press Enter to exit the program:");
        string endProgram = Console.ReadLine();
    
    }
}

public class KnapSackProblem
{
    public List<Item> FindItemsToBuy(List<Item> items, int budget, out int totalValue)
    {

        int[,] satisfaction = new int[items.Count + 1, budget + 1];
        bool[,] keep = new bool[items.Count + 1, budget + 1];

        // Creating the first knapsack tables (i.e. 1 satisfaction and 1 keep truth table)
        for (int i = 1; i <= items.Count; i++)
        {
            Item currentItem = items[i - 1];
            for (int value = 1; value <= budget; value++)
            {

                if (value >= currentItem.Price)
                {
                    int remainingValue = value - currentItem.Price;
                    int remainingSatisfactionValue = 0;
                    if (remainingValue > 0)
                    {
                        remainingSatisfactionValue = satisfaction[i - 1, remainingValue];
                    }
                    int currentItemTotalValue = currentItem.Satisfaction + remainingSatisfactionValue;
                    if (currentItemTotalValue >= satisfaction[i - 1, value])
                    {
                        keep[i, value] = true;
                        satisfaction[i, value] = currentItemTotalValue;
                    }
                    else
                    {
                        keep[i, value] = false;
                        satisfaction[i, value] = satisfaction[i - 1, value];
                    }
                }

                else
                {
                    satisfaction[i, value] = satisfaction[i - 1, value];
                }
            }
        }

        // Searching the satisfaction table for the highest satisfaction value
        // At the same time if the maximum satisfaction value is encountered, a sublist of 
        // items is created.
        // This sublist will then be added into the main list possibleItems
        List<List<Item>> possibleItems = new List<List<Item>>();

        int N, P, check = 0, maxSatisfaction = 0;
        for (int j = 1; j <= items.Count; j++)
        {
            for (int k = 1; k <= budget; k++)
            {

                if (satisfaction[j, k] >= maxSatisfaction)
                {
                    if (satisfaction[j, k] > maxSatisfaction)
                    {
                        maxSatisfaction = satisfaction[j, k];
                        possibleItems.Clear();
                        possibleItems.TrimExcess();
                    }

                    List<Item> sublist = new List<Item>();

                    N = j;
                    P = k;

                    while (N > 0)
                    {
                        if (keep[N, P])
                        {
                            sublist.Add(items[N - 1]);
                            P = P - items[N - 1].Price;
                        }
                        N--;
                    }

                    foreach (var sublist1 in possibleItems)
                    {
                        bool equalAB = sublist1.SequenceEqual(sublist); // Checking the main list possibleItems to avoid duplication
                        if (equalAB)                                    // before adding sublist to the main list
                        {                                  
                            check++;
                        }
                    }

                    if (check == 0)
                    {
                        possibleItems.Add(sublist);
                    }

                    else
                    {
                        check = 0;
                    }

                }
            }

            // Creating new knapsack tables to exclude the exception case where the 
            // total price and satisfaction value is the same
            int[,] satisfaction1 = new int[items.Count + 1, budget + 1];
            bool[,] keep1 = new bool[items.Count + 1, budget + 1];

            for (int i = 1; i <= items.Count; i++)
            {
                Item currentItem = items[i - 1];
                for (int value = 1; value <= budget; value++)
                {

                    if (value >= currentItem.Price)
                    {
                        int remainingValue = value - currentItem.Price;
                        int remainingSatisfactionValue = 0;
                        if (remainingValue > 0)
                        {
                            remainingSatisfactionValue = satisfaction1[i - 1, remainingValue];
                        }
                        int currentItemTotalValue = currentItem.Satisfaction + remainingSatisfactionValue;
                        if (currentItemTotalValue > satisfaction1[i - 1, value])
                        {
                            keep1[i, value] = true;
                            satisfaction1[i, value] = currentItemTotalValue;
                        }
                        else
                        {
                            keep1[i, value] = false;
                            satisfaction1[i, value] = satisfaction1[i - 1, value];
                        }
                    }

                    else
                    {
                        satisfaction1[i, value] = satisfaction1[i - 1, value];
                    }
                }
            }

            // Creating the sublist1 from the reedited Knapsack problem and adding this 
            // sublist1 into the same main list possibleItems (similar as above)
            for (int a = 1; a <= items.Count; a++)
            {
                for (int b = 1; b <= budget; b++)
                {

                    if (satisfaction1[a, b] == maxSatisfaction)
                    {
                        List<Item> sublist1 = new List<Item>();
                        N = a;
                        P = b;

                        while (N > 0)
                        {
                            if (keep1[N, P])
                            {
                                sublist1.Add(items[N - 1]);
                                P = P - items[N - 1].Price;
                            }
                            N--;
                        }

                        foreach (var sublist2 in possibleItems)
                        {
                            bool equalAB = sublist2.SequenceEqual(sublist1);
                            if (equalAB)
                            {
                                check++;
                            }
                        }

                        if (check == 0)
                        {
                            possibleItems.Add(sublist1);
                        }

                        else
                        {
                            check = 0;
                        }
                    }

                }
            }
        }

        // Print out the values in this nested list
        // (i.e. the different combinations for the same maximum satisfaction value
        // if there are more than 1 combination)
        Console.WriteLine();
        Console.WriteLine("Result: ");
        if (possibleItems.Count > 1)
        {
            Console.Write("There are " + possibleItems.Count + " combinations with maximum satisfaction ");
            Console.WriteLine(maxSatisfaction + ".");
            Console.WriteLine();
            int costprice, y = 1, z = 0;
            foreach (var sublist3 in possibleItems)
            {
                costprice = 0;
                Console.WriteLine("Combination " + y + ":");

                foreach (var value in sublist3)
                {
                    z++;
                    costprice = costprice + value.Price;
                    decimal d = new decimal(value.Price, 0, 0, false, 2);
                    Console.WriteLine(z + ") " + value.Name + " for $" + d);
                }

                decimal d1 = new decimal(costprice, 0, 0, false, 2);
                Console.WriteLine("Total spent: $" + d1);
                Console.WriteLine();
                y++;
                z = 0;
            }
        }

        else
        {
            if (maxSatisfaction == 0)
            {
                Console.WriteLine("Sorry, there are no available products you can buy.");
                Console.WriteLine("Please check your shopping list and budget.");
            }

            else
            {
                Console.Write("There is " + possibleItems.Count + " combination with maximum satisfaction ");
                Console.WriteLine(maxSatisfaction + ".");
                Console.WriteLine();
            }
        }

        // Finding out the best combination by using the bottom right most data on the keep truth table
        // (i.e. highest satisfaction value, low costs and lesser items)
        // This compiled list will then be returned to the main function
        List<Item> bestCombItems = new List<Item>();

        int remainBudget = budget;
        int item = items.Count;
        while (item > 0)
        {
            if (keep[item, remainBudget])
            {
                bestCombItems.Add(items[item - 1]);
                remainBudget = remainBudget - items[item - 1].Price;
            }
            item--;
        }

        totalValue = satisfaction[items.Count, budget];
        return bestCombItems;
    }
}

public class Item
{
    // Variables
    public String Name;
    public int Price;
    public int Satisfaction;
    public double TempPrice;

    // Constructor
    public Item(String ItemName, int ItemPrice, int ItemSatisfaction, double TempItemPrice)
    {
        this.Name = ItemName;
        this.Price = ItemPrice;
        this.Satisfaction = ItemSatisfaction;
        this.TempPrice = TempItemPrice;
    }

    // Overloading Constructor
    public Item(String ItemString)
    {
        String[] strsplit = ItemString.Split(' ');
        Name = strsplit[0];
        TempPrice = Convert.ToDouble(strsplit[1]);
        Price = Convert.ToInt32(TempPrice * 100);
        Satisfaction = Convert.ToInt32(strsplit[2]);
    }

    // Methods
    public override string ToString()
    {
        return "Name=" + Name + ",Price=" + Price + ",Satisfaction=" + Satisfaction;
    }

    public void printout()
    {
        decimal d = new decimal(Price, 0, 0, false, 2);
        Console.WriteLine(Name + " for $" + d);
    }
}