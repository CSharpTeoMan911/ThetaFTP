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

        public void MergeSortBySize(List<DirectoryItem>? items, bool ascending)
        {
            if (items?.Count > 1)
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
                            }
                            else
                            {
                                items[main_index] = right_l.ElementAt(right_index);
                                right_index++;
                            }
                            break;
                        case false:
                            if (left_l.ElementAt(left_index).size >= right_l.ElementAt(right_index).size)
                            {
                                items[main_index] = left_l.ElementAt(left_index);
                                left_index++;
                            }
                            else
                            {
                                items[main_index] = right_l.ElementAt(right_index);
                                right_index++;
                            }
                            break;
                    }

                    main_index++;
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

        public void MergeSortByName(List<DirectoryItem>? items, bool ascending)
        {
            if (items?.Count > 1)
            {
                int middle = items.Count / 2;

                List<DirectoryItem> left_l = new List<DirectoryItem>();
                List<DirectoryItem> right_l = new List<DirectoryItem>();

                for (int i = 0; i < middle; i++)
                    left_l.Add(items.ElementAt(i));

                for (int i = middle; i < items.Count; i++)
                    right_l.Add(items.ElementAt(i));

                MergeSortByName(left_l, ascending);
                MergeSortByName(right_l, ascending);



                int left_index = 0;
                int right_index = 0;
                int main_index = 0;

                while (left_index < left_l.Count && right_index < right_l.Count)
                {
                    char? left = left_l.ElementAt(left_index).name?[0];
                    char? right = right_l.ElementAt(right_index).name?[0];

                    if (left.HasValue && right.HasValue)
                    {
                        int left_value = 0;
                        alphabetic_order.TryGetValue(char.ToLower((char)left), out left_value);

                        int right_value = 0;
                        alphabetic_order.TryGetValue(char.ToLower((char)right), out right_value);

                        switch (ascending)
                        {
                            case true:
                                if (left_value <= right_value)
                                {
                                    items[main_index] = left_l.ElementAt(left_index);
                                    left_index++;
                                }
                                else
                                {
                                    items[main_index] = right_l.ElementAt(right_index);
                                    right_index++;
                                }
                                break;
                            case false:
                                if (left_value >= right_value)
                                {
                                    items[main_index] = left_l.ElementAt(left_index);
                                    left_index++;
                                }
                                else
                                {
                                    items[main_index] = right_l.ElementAt(right_index);
                                    right_index++;
                                }
                                break;
                        }

                        main_index++;
                    }
                    else
                    {
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

    }
}
