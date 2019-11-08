using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace ScenarioWriter
{
    public partial class MainWindow : Window
    {
        BindingList<Scenario> Scenarios = new BindingList<Scenario>();

        object selectedItem = typeof(Nullable);


        public MainWindow()
        {
            InitializeComponent();

            treeviewList.ItemsSource = Scenarios;
            //LoadTestData();
            //LoadTestData2();
        }

        private void AddScenario_Click(object sender, RoutedEventArgs e)
        {
            Scenarios.Add(new Scenario()
            {
                ScenarioNumber = Scenarios.Count + 1,
                ScenarioDescription = "New Empty Scenario",
                Steps = new BindingList<Step>()
            });
        }

        private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem != null)
            {

                if (selectedItem.GetType() == typeof(Scenario))
                {
                    Scenario s = (Scenario)selectedItem;

                    s.Steps.Add(new Step()
                    {
                        Parent = selectedItem,
                        StepNumber = s.Steps.Count() + 1,
                        StepDescription = "New Empty Step",
                        ActionName = "",
                        IsConditional = false,
                        FulfilledConditionalFlow = "continue",
                        FulfilledTarget = "",
                        UnfulfilledConditionalFlow = "-",
                        UnfulfilledTarget = "",
                        SubSteps = new BindingList<Step>()
                    });
                }
                else if (selectedItem.GetType() == typeof(Step))
                {
                    Step s = (Step)selectedItem;

                    s.SubSteps.Add(new Step()
                    {
                        Parent = selectedItem,
                        StepNumber = s.SubSteps.Count() + 1,
                        StepDescription = "New Empty Step",
                        ActionName = "",
                        IsConditional = false,
                        FulfilledConditionalFlow = "continue",
                        FulfilledTarget = "",
                        UnfulfilledConditionalFlow = "-",
                        UnfulfilledTarget = "",
                        SubSteps = new BindingList<Step>()
                    });
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem.GetType() == typeof(Scenario))
            {
                Scenario selected = (Scenario)selectedItem;
                int selectedNumber = selected.ScenarioNumber - 1;
                Scenarios.Remove(selected);

                for (int i = selectedNumber; i < Scenarios.Count; i++)
                {
                    Scenarios[i].ScenarioNumber -= 1;
                }
            }
            else if (selectedItem.GetType() == typeof(Step))
            {
                Step selected = (Step)selectedItem;
                int selectedNumber = selected.StepNumber - 1;

                if (selected.Parent.GetType().Equals(typeof(Scenario)))
                {
                    //Remove Step
                    Scenario parent = (Scenario)selected.Parent;
                    parent.Steps.Remove(selected);

                    for (int i = selectedNumber; i < parent.Steps.Count; i++)
                    {
                        parent.Steps[i].StepNumber -= 1;
                    }
                }
                else if (selected.Parent.GetType().Equals(typeof(Step)))
                {
                    //Remove Substep
                    Step parent = (Step)selected.Parent;
                    parent.SubSteps.Remove(selected);

                    for (int i = selectedNumber; i < parent.SubSteps.Count; i++)
                    {
                        parent.SubSteps[i].StepNumber -= 1;
                    }
                }
            }
            RefreshTreeView();
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem.GetType() == typeof(Scenario))
            {
                Scenario selected = (Scenario)selectedItem;
                int selectedNumber = selected.ScenarioNumber - 1;
                if (selectedNumber == 0)
                    return;
                Scenarios[selectedNumber - 1].ScenarioNumber += 1;
                selected.ScenarioNumber -= 1;

                Swap(Scenarios, selectedNumber - 1, selectedNumber);
            }
            else if (selectedItem.GetType() == typeof(Step))
            {
                Step selected = (Step)selectedItem;
                int selectedNumber = selected.StepNumber - 1;
                if (selectedNumber == 0)
                    return;
                selected.StepNumber -= 1;

                if (selected.Parent.GetType().Equals(typeof(Scenario)))
                {
                    Scenario parent = (Scenario)selected.Parent;
                    parent.Steps[selectedNumber - 1].StepNumber += 1;
                    Swap(parent.Steps, selectedNumber - 1, selectedNumber);
                }
                else if (selected.Parent.GetType().Equals(typeof(Step)))
                {
                    Step parent = (Step)selected.Parent;
                    parent.SubSteps[selectedNumber - 1].StepNumber += 1;
                    Swap(parent.SubSteps, selectedNumber - 1, selectedNumber);
                }
            }
            RefreshTreeView();
        }
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem.GetType() == typeof(Scenario))
            {
                Scenario selected = (Scenario)selectedItem;
                int selectedNumber = selected.ScenarioNumber - 1;
                if (selectedNumber >= Scenarios.Count - 1)
                    return;

                Scenarios[selectedNumber + 1].ScenarioNumber -= 1;
                selected.ScenarioNumber += 1;

                Swap(Scenarios, selectedNumber + 1, selectedNumber);
            }
            else if (selectedItem.GetType() == typeof(Step))
            {
                Step selected = (Step)selectedItem;
                int selectedNumber = selected.StepNumber - 1;

                if (selected.Parent.GetType().Equals(typeof(Scenario)))
                {
                    Scenario parent = (Scenario)selected.Parent;
                    if (selectedNumber >= parent.Steps.Count - 1)
                        return;
                    selected.StepNumber += 1;
                    parent.Steps[selectedNumber + 1].StepNumber -= 1;
                    Swap(parent.Steps, selectedNumber + 1, selectedNumber);
                }
                else if (selected.Parent.GetType().Equals(typeof(Step)))
                {
                    Step parent = (Step)selected.Parent;
                    if (selectedNumber >= parent.SubSteps.Count - 1)
                        return;
                    selected.StepNumber += 1;
                    parent.SubSteps[selectedNumber + 1].StepNumber -= 1;
                    Swap(parent.SubSteps, selectedNumber + 1, selectedNumber);
                }
            }
            RefreshTreeView();
        }

        private ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as ItemsControl;
        }

        private static void Swap<T>(BindingList<T> list, int first, int second)
        {
            T temp = list[first];
            list[first] = list[second];
            list[second] = temp;
        }

        private void RefreshTreeView()
        {
            CollectionViewSource.GetDefaultView(treeviewList.ItemsSource).Refresh();
        }

        private void ExportXml_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml file (*.xml)|*.xml";
            saveFileDialog.ShowDialog();


            if (saveFileDialog.FileName != "")
            {
                XmlTextWriter textWriter = new XmlTextWriter(saveFileDialog.FileName, System.Text.Encoding.UTF8);
                textWriter.WriteStartDocument();
                textWriter.WriteRaw("\r\n");
                textWriter.Formatting = Formatting.Indented;
                textWriter.WriteStartElement("root");

                textWriter.WriteStartElement("UserStory");
                textWriter.WriteAttributeString("Text", UserStoryTextBox.Text);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("Scenarios");

                // save the nodes, using recursive method
                SaveScenarios(treeviewList.Items, textWriter);

                textWriter.WriteEndElement();

                textWriter.Close();
            }
        }

        private void SaveScenarios(ItemCollection nodesCollection, XmlTextWriter textWriter)
        {

            for (int i = 0; i < nodesCollection.Count; i++)
            {
                Scenario scenario = (Scenario)nodesCollection[i];
                textWriter.WriteStartElement("Scenario");

                textWriter.WriteAttributeString("ScenarioNumber", scenario.ScenarioNumber.ToString());
                textWriter.WriteAttributeString("ScenarioDescription", scenario.ScenarioDescription);

                if (scenario.Steps.Count > 0)
                {
                    textWriter.WriteStartElement("Steps");
                    SaveSteps(scenario.Steps, textWriter);
                    textWriter.WriteEndElement();

                }

                textWriter.WriteEndElement();
            }
        }

        private void SaveSteps(BindingList<Step> stepsCollection, XmlTextWriter textWriter)
        {
            for (int i = 0; i < stepsCollection.Count; i++)
            {
                Step step = (Step)stepsCollection[i];

                textWriter.WriteStartElement("Step");

                textWriter.WriteAttributeString("StepNumber", step.StepNumber.ToString());
                textWriter.WriteAttributeString("StepDescription", step.StepDescription);
                textWriter.WriteAttributeString("ActionName", step.ActionName);
                textWriter.WriteAttributeString("IsConditional", step.IsConditional.ToString());
                textWriter.WriteAttributeString("FulfilledConditionalFlow", step.FulfilledConditionalFlow);
                textWriter.WriteAttributeString("FulfilledTarget", step.FulfilledTarget);
                textWriter.WriteAttributeString("UnfulfilledConditionalFlow", step.UnfulfilledConditionalFlow);
                textWriter.WriteAttributeString("UnfulfilledTarget", step.UnfulfilledTarget);

                if (step.SubSteps.Count > 0)
                {
                    textWriter.WriteStartElement("SubSteps");
                    SaveSteps(step.SubSteps, textWriter);
                    textWriter.WriteEndElement();
                }

                textWriter.WriteEndElement();
            }
        }


        private void ImportXml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Xml file (*.xml)|*.xml";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                Scenarios = new BindingList<Scenario>();
                XmlTextReader reader = new XmlTextReader(openFileDialog.FileName);

                object currentParent = typeof(Nullable);
                bool isCurrentParentStep = false;
                try
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "UserStory")
                            {
                                UserStoryTextBox.Text = reader.GetAttribute("Text");
                            }
                            if (reader.Name == "Scenario")
                            {
                                Scenario newScenario = new Scenario()
                                {
                                    ScenarioNumber = Scenarios.Count + 1,
                                    ScenarioDescription = reader.GetAttribute("ScenarioDescription"),
                                    Steps = new BindingList<Step>()
                                };

                                currentParent = newScenario;
                                isCurrentParentStep = false;
                                Scenarios.Add(newScenario);
                            }

                            if (reader.Name == "Step")
                            {
                                int stepNumber = -1;
                                if (isCurrentParentStep)
                                {
                                    Step parentStep = (Step)currentParent;
                                    stepNumber = parentStep.SubSteps.Count() + 1;
                                }
                                else
                                {
                                    Scenario parentScenario = (Scenario)currentParent;
                                    stepNumber = parentScenario.Steps.Count() + 1;
                                }

                                Step newStep = new Step()
                                {
                                    Parent = currentParent,
                                    StepNumber = stepNumber,
                                    StepDescription = reader.GetAttribute("StepDescription"),
                                    ActionName = reader.GetAttribute("ActionName"),
                                    IsConditional = bool.Parse(reader.GetAttribute("IsConditional")),
                                    FulfilledConditionalFlow = reader.GetAttribute("FulfilledConditionalFlow"),
                                    FulfilledTarget = reader.GetAttribute("FulfilledTarget"),
                                    UnfulfilledConditionalFlow = reader.GetAttribute("UnfulfilledConditionalFlow"),
                                    UnfulfilledTarget = reader.GetAttribute("UnfulfilledTarget"),
                                    SubSteps = new BindingList<Step>()
                                };

                                if (isCurrentParentStep)
                                {
                                    Step parentStep = (Step)currentParent;
                                    parentStep.SubSteps.Add(newStep);
                                }
                                else
                                {
                                    Scenario parentScenario = (Scenario)currentParent;
                                    parentScenario.Steps.Add(newStep);
                                }
                            }
                            if (reader.Name == "SubSteps")
                            {
                                Scenario previousParent = (Scenario)currentParent;
                                currentParent = previousParent.Steps[previousParent.Steps.Count() - 1];
                                isCurrentParentStep = true;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (reader.Name == "SubSteps")
                            {
                                Step previousParent = (Step)currentParent;
                                currentParent = previousParent.Parent;
                                isCurrentParentStep = false;
                            }
                        }

                    }
                }
                finally
                {
                    reader.Close();
                }

                treeviewList.ItemsSource = Scenarios;
                RefreshTreeView();
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            TreeView tv = (TreeView)sender;
            selectedItem = tv.SelectedItem;
        }

        //eksperymentalna automatyzacja po słowach kluczowych
        private void Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var textBox = sender as TextBox;

            //if (textBox.DataContext.GetType().Equals(typeof(Scenario)))
            //{
            //    Scenario scenario = textBox.DataContext as Scenario;
            //    scenario.ScenarioDescription = textBox.Text;
            //    return;
            //}
            //else if (textBox.DataContext.GetType().Equals(typeof(Step)))
            //{
            //    Step step = textBox.DataContext as Step;

            //    string description = textBox.DataContext as string;
            //    description = textBox.Text;

            //    List<string> singleKeywords = new List<string>() { "powtarzaj", "jeżeli" };
            //    List<string> tooltipSingleKeywords = new List<string>() { "repeat", "if" };

            //    List<string> doubleKeywords = new List<string>() { "przejdź", "do", "idź", "do" };
            //    List<string> tooltipDoubleKeywords = new List<string>() { "go", "to", "go", "to" };
            //    char[] delimiterChars = { ' ', ',', '.', ':', '\t', '<', '>' };
            //    string[] words = description.Split(delimiterChars);

            //    for (int i = 0; i < words.Length; i++)
            //    {
            //        for (int j = 0; j < singleKeywords.Count; j++)
            //        {
            //            if (words[i].Equals(singleKeywords[j]))
            //            {
            //                step.ActionName = tooltipSingleKeywords[j] + " ";
            //            }
            //        }

            //        for (int j = 0; j < doubleKeywords.Count; j += 2)
            //        {
            //            if (words[i].Equals(doubleKeywords[j]) && (i + 1 < words.Length))
            //            {
            //                if (words[i + 1].Equals(doubleKeywords[j + 1]))
            //                {
            //                    step.ActionName = tooltipDoubleKeywords[j] + " " + tooltipDoubleKeywords[j + 1] + " ";
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void Fulfilled_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void IsConditional_CheckBoxChanged(object sender, RoutedEventArgs e)
        {

        }

        #region Test Data

        private void LoadTestData()
        {
            Scenario myScenario = new Scenario()
            {
                ScenarioNumber = Scenarios.Count + 1,
                ScenarioDescription = "Buy a product",
                Steps = new BindingList<Step>()
            };

            BindingList<Step> mySteps = new BindingList<Step>();

            Step myStep1 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "Użytkownik wybiera przedmiot do kupienia",
                ActionName = "AddItemToWallet",
                SubSteps = new BindingList<Step>(),
                IsConditional = true,
                FulfilledConditionalFlow = "continue"
            };
            mySteps.Add(myStep1);

            Step myStep2 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "Użytkownik przechodzi do kasy",
                ActionName = "CheckOut",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep2);

            Step myStep3 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "Użytkownik wypełnia informację wysyłkowe",
                ActionName = "GetShippingInfo",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep3);

            Step myStep4 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "System przedstawia podsumowanie transakcji",
                ActionName = "ShowTransactionDetails",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep4);

            Step myStep5 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "Użytkownik wypełnia dane karty płatniczej",
                ActionName = "GetCardInfo",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep5);

            Step myStep6 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "System autoryzuje płatność",
                ActionName = "AuthorizePurchase",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep6);

            Step mySubStep61 = new Step()
            {
                Parent = myStep6,
                StepNumber = myStep6.SubSteps.Count + 1,
                StepDescription = "Dane prawidłowe",
                ActionName = "InvalidCardData",
                SubSteps = new BindingList<Step>()
            };
            myStep6.SubSteps.Add(mySubStep61);

            Step myStep7 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "System akceptuje sprzedaż",
                ActionName = "ConfirmSale",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep7);

            Step myStep8 = new Step()
            {
                Parent = myScenario,
                StepNumber = mySteps.Count + 1,
                StepDescription = "System wysyła maila z potwierdzeniem zakupu",
                ActionName = "SendConfirmationEmail",
                SubSteps = new BindingList<Step>()
            };
            mySteps.Add(myStep8);

            myScenario.Steps = mySteps;
            Scenarios.Add(myScenario);
        }

        private void LoadTestData2()
        {

            Scenarios.Add(new Scenario()
            {
                ScenarioNumber = Scenarios.Count + 1,
                ScenarioDescription = "Buy a product",
                Steps = new BindingList<Step>()
                {
                    new Step(){ StepNumber = 1 , StepDescription = "Przykladowy krok 1", ActionName = "Multiply", SubSteps = new BindingList<Step>() },
                    new Step(){ StepNumber = 2 , StepDescription = "Przykladowy krok 2", ActionName = "Divide", SubSteps = new BindingList<Step>() },
                    new Step(){ StepNumber = 3 , StepDescription = "Przykladowy krok 3", ActionName = "Subtract", SubSteps = new BindingList<Step>() },
                }
            });

            Scenarios.Add(new Scenario()
            {
                ScenarioNumber = Scenarios.Count + 1,
                ScenarioDescription = "Example scenario",
                Steps = new BindingList<Step>()
                {
                    new Step(){ StepNumber = 1 , StepDescription = "Example step 1", ActionName = "Multiply", SubSteps = new BindingList<Step>() },
                    new Step(){ StepNumber = 2 , StepDescription = "Example step 2", ActionName = "Divide", SubSteps = new BindingList<Step>() },
                    new Step(){ StepNumber = 3 , StepDescription = "Example step 3", ActionName = "Subtract",
                        SubSteps = new BindingList<Step>()
                        {
                            new Step() { StepNumber = 1 , StepDescription = "Example substep 1", ActionName = "Multiply", SubSteps = new BindingList<Step>() },
                            new Step() { StepNumber = 2 , StepDescription = "Example substep 2", ActionName = "Subtract", SubSteps = new BindingList<Step>() },
                        },
                    }
                }
            });
        }
        #endregion
    }
}
