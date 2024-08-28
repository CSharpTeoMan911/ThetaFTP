using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class SortUtility
    {
        private Dictionary<char, int> alphabetic_order = new Dictionary<char, int>();

        public SortUtility()
        {
            int index = 0;
            "abcdefghijklmnopqrstuvwxyz0123456789".ToList().ForEach(character => {
                alphabetic_order.Add(character, index);
                index++;
            });
        }

        public void MergeSortBySize(List<DirectoryItem> items, bool ascending)
        {
            if (items.Count > 1)
            {
                int middle = items.Count / 2;

                List<DirectoryItem> left_l = new List<DirectoryItem>();
                List<DirectoryItem> right_l = new List<DirectoryItem>();

                for (int i = 0; i < middle; i++)
                    left_l.Add(items.ElementAt(i));

                for (int i = middle; i < items.Count; i++)
                    right_l.Add(items.ElementAt(i));

                MergeSortBySize(left_l, ascending);
                MergeSortBySize(right_l, ascending);


                
                int left_index = 0;
                int right_index = 0;
                int main_index = 0;

                while (left_index < left_l.Count && right_index < right_l.Count)
                {
                    switch (ascending)
                    {
                        case true:
                            if (left_l.ElementAt(left_index).size <= right_l.ElementAt(right_index).size)
                            {
                                items[main_index] = left_l.ElementAt(left_index);
                                left_index++;
                                main_index++;
                            }
                            else
                            {
                                items[main_index] = right_l.ElementAt(right_index);
                                right_index++;
                                main_index++;
                            }
                            break;
                        case false:
                            if (left_l.ElementAt(left_index).size >= right_l.ElementAt(right_index).size)
                            {
                                items[main_index] = left_l.ElementAt(left_index);
                                left_index++;
                                main_index++;
                            }
                            else
                            {
                                items[main_index] = right_l.ElementAt(right_index);
                                right_index++;
                                main_index++;
                            }
                            break;
                    }
                }

                while (left_index < left_l.Count)
                {
                    items[main_index] = left_l.ElementAt(left_index);
                    left_index++;
                    main_index++;
                }



                while (right_index < right_l.Count)
                {
                    items[main_index] = right_l.ElementAt(right_index);
                    right_index++;
                    main_index++;
                }
            }
        }

        public void MergeSortByName(List<DirectoryItem> items, bool ascending)
        {
            if (items.Count > 1)
            {
                int middle = items.Count / 2;

                List<DirectoryItem> left_l = new List<DirectoryItem>();
                List<DirectoryItem> right_l = new List<DirectoryItem>();

                for (int i = 0; i < middle; i++)
                    left_l.Add(items.ElementAt(i));

                for (int i = middle; i < items.Count; i++)
                    right_l.Add(items.ElementAt(i));

                MergeSortBySize(left_l, ascending);
                MergeSortBySize(right_l, ascending);



                int left_index = 0;
                int right_index = 0;
                int main_index = 0;

                while (left_index < left_l.Count && right_index < right_l.Count)
                {
                    if (left_l.ElementAt(left_index).name != null && right_l.ElementAt(right_index).name != null)
                    {
                        #pragma warning disable CS8602 // Dereference of a possibly null reference.
                        char left_c = left_l.ElementAt(left_index).name[0];
                        char right_c = right_l.ElementAt(right_index).name[0];

                        Console.WriteLine("L Val W: " + left_l.ElementAt(left_index).name);

                        Console.WriteLine("R Val W: " + right_l.ElementAt(right_index).name);

                        int l_val = 0;
                        alphabetic_order.TryGetValue(left_c, out l_val);

                        int r_val = 0;
                        alphabetic_order.TryGetValue(right_c, out r_val);

                        switch (ascending)
                        {
                            case true:
                                if (l_val <= r_val)
                                {
                                    items[main_index] = left_l.ElementAt(left_index);
                                    Console.WriteLine("Val: " + items[main_index].name[0]);
                                    Console.WriteLine("Val w: " + items[main_index].name);
                                    left_index++;
                                    main_index++;
                                }
                                else
                                {
                                    items[main_index] = right_l.ElementAt(right_index);
                                    Console.WriteLine("Val: " + items[main_index].name[0]);
                                    Console.WriteLine("Val w: " + items[main_index].name);
                                    right_index++;
                                    main_index++;
                                }
                                break;
                            case false:
                                if (l_val >= r_val)
                                {
                                    items[main_index] = left_l.ElementAt(left_index);
                                    Console.WriteLine("Val: " + items[main_index].name[0]);
                                    Console.WriteLine("Val w: " + items[main_index].name);
                                    left_index++;
                                    main_index++;
                                }
                                else
                                {
                                    items[main_index] = right_l.ElementAt(right_index);
                                    Console.WriteLine("Val: " + items[main_index].name[0]);
                                    Console.WriteLine("Val w: " + items[main_index].name);
                                    right_index++;
                                    main_index++;
                                }
                                break;
                        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    }
                    else
                    {
                        Console.WriteLine("!!! NULL !!!");
                        break;
                    }
                }

                while (left_index < left_l.Count)
                {
                    items[main_index] = left_l.ElementAt(left_index);
                    left_index++;
                    main_index++;

                    Console.WriteLine("Val: " + items[main_index].name[0]);
                }



                while (right_index < right_l.Count)
                {
                    items[main_index] = right_l.ElementAt(right_index);
                    right_index++;
                    main_index++;

                    Console.WriteLine("Val: " + items[main_index].name[0]);
                }

            }

            Console.WriteLine("\n\n\n\n");
        }

    }
}
