using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Markup;

namespace GUIDatabaseTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SimpleDataSource dataSource = new SimpleDataSource("147.143.75.11", "edu030", 3306, "edu030", "74920");
        //fields for the assigning of textbox text values for Agents and Missions
        string missionName, missionDescr, missionCountry, missionCity, stringInput;
        string agentName, agentAge, agentGender, agentAlias, agentSecurityClearance, agentDeptID, agentJobID, agentMissionID;
        //arrays to hold colum datas to be loaded at different sections of the application on the combo boxes
        string[] tableArray = { "Agent_Name", "Agent_Age", "Agent_Gender", "Agent_Alias", "Security_Clearance", "Dept_ID", "Job_ID", "Mission_ID" };
        string[] attributeArray = { "Mission_Name", "Mission_Description", "Mission_Country", "Mission_City" };
        string[] jobsArray = { "Job_Name", "Job_Description" };
        string[] deptArray = { "Dept_Name", "Dept_Address", "Dept_Tel" };
        string[] keys, values;

        public MainWindow()
        {
            InitializeComponent();
            //filling up combo boxes under ADD Agent
            fillCombo("SELECT * FROM Departments;", "Dept_ID", comboBoxDeptID);
            fillCombo("SELECT * FROM Jobs;", "Job_ID", comboBoxJobID);
            fillCombo("SELECT * FROM Missions;", "Mission_ID", comboBoxMissionID);
            //array to hold combo boxes at Join Query for Agent data
            ComboBox[] agentColumn = { comboq1, comboq2, comboq3, comboq4 };
            //filling up the agent texboxes under Join Query with information from database
            for (int x = 0; x < agentColumn.Length; x++)
            {
                fillComboColumn("SELECT * FROM Agents;", agentColumn[x]);
            }
        }

        /// <summary>
        /// for user to add new Agent data to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Agent_Insert(object sender, RoutedEventArgs e)
        {
            //setting up variables for textboxes
            agentName = textbox_agentName.Text;
            agentAge = textbox_agentAge.Text;
            agentGender = comboBoxGender.Text;
            agentAlias = textbox_agentAlias.Text;
            agentSecurityClearance = textbox_agentSecurityClearance.Text;
            agentDeptID = comboBoxDeptID.Text;
            agentJobID = comboBoxJobID.Text;
            agentMissionID = comboBoxMissionID.Text;
            //make the user fill up the IDs linking the agent table to other tables. 
            if (agentDeptID == "" || agentJobID == "" || agentMissionID == "")
            {
                MessageBox.Show("You have to choose a value for all the IDs");
            }
            else
            {
                try
                {
                    //setting up string to check for only the entries where they can type in information
                    stringInput = agentName + agentAge + agentAlias + agentSecurityClearance;
                    //setup query string
                    string insert = @"INSERT INTO Agents (Agent_Name, Agent_Age, Agent_Gender, Agent_Alias, Security_Clearance, Dept_ID, Job_ID, Mission_ID) 
                            VALUES (@agentName, @agentAge, @agentGender, @agentAlias, @securityClearance, @agentDeptID, @agent_JobID, @agent_MissionID)";
                    //prepare
                    keys = new string[] { @"@agentName", "@agentAge", "@agentGender", "@agentAlias", 
                                        "@securityClearance", "@agentDeptID", "@agent_JobID", "@agent_MissionID"};
                    values = new string[] { agentName, agentAge, agentGender, agentAlias, 
                                            agentSecurityClearance, agentDeptID, agentJobID, agentMissionID };
                    //perform insertion if the query is valid else display the information on the error.
                    if (LegalString(stringInput) && isNumeric(agentAge) && isNumeric(agentSecurityClearance))
                    {
                        dataSource.UpdatePrepare(insert, keys, values);
                        MessageBox.Show(agentName + " Details Added");
                    }
                    else
                    {
                        MessageBox.Show(@"Please check the following. 
                        1. Fill all boxes and only enter alphabets and numbers.
                        2. Age and Security Clearance has to be whole numbers.
                        3. Edit any entries in red and try again.");
                    }
                }
                catch (MySqlException err)
                {
                    err.StackTrace.ToString();
                    MessageBox.Show("Not connected to database. Unable to add information.");
                }
            }
        }

        /// <summary>
        /// for user to add new missions data to the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Mission_Insert(object sender, RoutedEventArgs e)
        {
            //setting up variables for the textboxes
            missionName = textbox_missionName.Text;
            missionDescr = textbox_missionDescr.Text;
            missionCountry = textbox_missionCountry.Text;
            missionCity = textbox_missionCity.Text;
            //setting up string to be used to check for validity
            stringInput = missionName + missionDescr + missionCountry + missionCity;
            //setting up query string
            string insert = "INSERT INTO Missions (Mission_Name, Mission_Description, Mission_Country, Mission_City) VALUES (@name, @description, @country, @city)";
            //prepare
            keys = new string[] { "@name", "@description", "@country", "@city" };
            values = new string[] { missionName, missionDescr, missionCountry, missionCity };
            //perform insertion if query is valid and all fields have data entered. Else display information on the error. 
            try
            {
                if (LegalString(stringInput) && (missionName != "" && missionDescr != "" && missionCountry != "" && missionCity != ""))
                {
                    dataSource.UpdatePrepare(insert, keys, values);
                    comboBoxMissionID.Items.Clear();
                    fillCombo("SELECT * FROM Missions;", "Mission_ID", comboBoxMissionID);
                    MessageBox.Show(missionName + " Details Added");
                }
                else
                {
                    MessageBox.Show(@"Please check the following. 
                    1. Enter information for all fields.
                    2. Enter only alphabets and numbers.
                    3. Edit any entries in red and try again.");
                }
            }
            catch (MySqlException err)
            {
                err.StackTrace.ToString();
                MessageBox.Show("Not connected to database. Unable to add information.");
            }
        }

        /// <summary>
        /// displays all columns and attributes in the Missions table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mission_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Missions;";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// displays all columns and attributes in the Agents table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Agent_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Agents;";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// displays all columns and attributes in the Departments table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Department_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Departments;";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// displays all columns and attributes in the Jobs table 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Job_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Jobs;";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }

        /// <summary>
        /// used to count and display the total number of missions available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void countTotalMissions_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT COUNT(*) AS Total_No_Of_Active_Missions FROM Missions";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// used to count and display the total number of agents available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void countAgents_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT COUNT(*) AS Total_No_Of_Active_Agents FROM Agents";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// used to display the agents within the selected bandwidth of security clearance 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void securityClassAgent_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT * FROM Agents WHERE Security_Clearance >'" + Math.Round(slider1.Value) + "'";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }

        /// <summary>
        /// used to display a collaborated information on the relation between agents table and missions table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mission_Assignments_Click(object sender, RoutedEventArgs e)
        {
            string query = @"SELECT Agents.Agent_Name, Agents.Agent_Age, Agents.Agent_Gender, 
                            Agents.Agent_Alias, Missions.Mission_Name, Missions.Mission_Description, 
                            Missions.Mission_Country, Missions.Mission_City
                            FROM Agents INNER JOIN Missions ON 
                            Agents.Mission_ID=Missions.Mission_ID
                            ORDER BY Agents.Agent_Name";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// used to display a collaborated information on the relation between agents table and jobs table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void job_Assignments_Click(object sender, RoutedEventArgs e)
        {
            string query = @"SELECT Agents.Agent_Name, Agents.Agent_Age, Agents.Agent_Gender, 
                            Agents.Agent_Alias, Agents.Security_Clearance, Jobs.Job_Name, Jobs.Job_Description
                            FROM Agents INNER JOIN Jobs ON Agents.Job_ID=Jobs.Job_ID ORDER BY Jobs.Job_Name";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }
        /// <summary>
        /// used to display a collaborated information on the relation between agents table and departments table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dept_Assignments_Click(object sender, RoutedEventArgs e)
        {
            string query = @"SELECT Agents.Agent_Name, Agents.Agent_Age, Agents.Agent_Gender, Agents.Agent_Alias, Agents.Security_Clearance, 
                            Departments.Dept_Name, Dept_Address, Dept_Tel
                            FROM Agents INNER JOIN Departments ON Agents.Dept_ID=Departments.Dept_ID 
                            ORDER BY Departments.Dept_Name";
            dataSource.Query(query);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }

        /// <summary>
        /// Retrieve data from the database and display on the datagrid based on the user's choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Retrieve(object sender, RoutedEventArgs e)
        {
            //setting up local variables for textboxes
            string retrieveData = textBoxRetrieveData.Text;
            string table = combo1.Text;
            string attribute = combo2.Text;
            //setting up retrieve query string
            string query = "SELECT * FROM " + table + " WHERE " + attribute + "='" + retrieveData + "'";
            //if it is a valid query, perform the query and update the datagrid. else display the validity details. 
            if (LegalString(retrieveData))
            {
                dataSource.Query(query);
                dataGrid1.ItemsSource = dataSource.dt.DefaultView;
            }
            else
            {
                MessageBox.Show("Invalid Entry. Enter only numbers and alphabets.");
            }
        }

        /// <summary>
        /// Delete data from the database depending on the user's choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            //setting up local variables for textboxes
            string name = textbox_deleteName.Text;
            string table = comboBoxDeleteTable.Text;
            //id to toggle between user choice of Agents or Missions
            string id;
            if (table == "Agents")
            {
                id = "Agent_Name";
            }
            else
            {
                id = "Mission_Name";
            }
            //setting up delete query string
            string delete = "DELETE FROM " + table + " WHERE " + id + "=@name";
            //prepare
            string[] keys = new string[] { "@name" };
            string[] values = new string[] { name };

            //if it is a valid query, perform the deletion else display the validity details. 
            try
            {
                if (LegalString(name))
                {
                    dataSource.UpdatePrepare(delete, keys, values);
                    if (table == "Missions")
                    {
                        comboBoxMissionID.Items.Clear();
                        fillCombo("SELECT * FROM Missions;", "Mission_ID", comboBoxMissionID);
                    }
                    MessageBox.Show(name + " Details deleted");
                }
                else
                {
                    MessageBox.Show("Invalid Entry. Enter only numbers and alphabets.");
                }
            }
            catch (Exception err)
            {
                err.StackTrace.ToString();
                MessageBox.Show(@"Unable to perform deletion. Please check the following.
                1. You can't delete a mission if it is currently assigned to an agent.
                2. Check if you are connected to the database and try again.");
            }
        }

        /// <summary>
        /// used to make a selective query involving the relation between 2 tables and display it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joinQueryButton_Click(object sender, RoutedEventArgs e)
        {
            string queryValue = string.Empty;
            string q;
            try
            {
                //this is when the secondary table is not chosen or no values set for it.
                //performing checks on the combobox text values for secondary table prior to adding.
                //allows user to use single/multiple/varied combo box choices this way. 
                if (secondaryTable.Text.Equals(string.Empty) || (stCombo1.Text.Equals(string.Empty) && stCombo2.Text.Equals(string.Empty)))
                {
                    if (!comboq1.Text.Equals(string.Empty))
                    { queryValue += comboq1.Text + ","; }
                    if (!comboq2.Text.Equals(string.Empty))
                    { queryValue += comboq2.Text + ","; }
                    if (!comboq3.Text.Equals(string.Empty))
                    { queryValue += comboq3.Text + ","; }
                    if (!comboq4.Text.Equals(string.Empty))
                    { queryValue += comboq4.Text + ","; }
                    //removing the last char in the query which will be a semi-colon
                    queryValue = queryValue.Substring(0, queryValue.Length - 1);
                }
            }
            catch (ArgumentOutOfRangeException err)
            {
                err.StackTrace.ToString();
            }
            //setting up query with the checked string where there is no value for secondary table
            q = "SELECT " + queryValue + " FROM Agents;";

            //this is where secondary table has values
            if (!secondaryTable.Text.Equals(string.Empty) && (!stCombo1.Text.Equals(string.Empty) || !stCombo2.Text.Equals(string.Empty)))
            {
                //performing checks on the combobox text values prior to adding.
                //allows user to use single/multiple/varied combo box choices this way. 
                if (!comboq1.Text.Equals(string.Empty))
                { queryValue += "Agents." + comboq1.Text + ","; }
                if (!comboq2.Text.Equals(string.Empty))
                { queryValue += "Agents." + comboq2.Text + ","; }
                if (!comboq3.Text.Equals(string.Empty))
                { queryValue += "Agents." + comboq3.Text + ","; }
                if (!comboq4.Text.Equals(string.Empty))
                { queryValue += "Agents." + comboq4.Text + ","; }
                if (!stCombo1.Text.Equals(string.Empty))
                {
                    queryValue += secondaryTable.Text + ".";
                    queryValue += stCombo1.Text + ",";
                }
                if (!stCombo2.Text.Equals(string.Empty))
                {
                    queryValue += secondaryTable.Text + ".";
                    queryValue += stCombo2.Text + ",";
                }
                //removing the last char in the query which will be a semi-colon
                queryValue = queryValue.Substring(0, queryValue.Length - 1);
                //setting up query with the checked string
                //the hiddentext.content is the value of the label hidden from user. At the backgground it is
                //dat binded and gives us the id value depending on which secondary table is chosen. 
                q = @"SELECT " + queryValue + " FROM Agents INNER JOIN " + secondaryTable.Text +
                    " ON Agents." + hiddenText.Content + "=" + secondaryTable.Text + "." + hiddenText.Content; ;
            }
            dataSource.Query(q);
            dataGrid1.ItemsSource = dataSource.dt.DefaultView;
        }

        /// <summary>
        /// used to perform updates to the data in the database depending on the user's input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Update(object sender, RoutedEventArgs e)
        {
            //assigning local variables to  text values in combo boxes 
            string table = combo3.Text;
            string attr = combo4.Text;
            string current = combo7.Text;
            string input = textbox_update.Text;
            string currentAttr = combo6.Text;
            //setting up the query string
            string update = "UPDATE " + table + " SET " + attr + "='" + @input + "' WHERE " + currentAttr + "='" + current + "'"; ;
            //prepare
            string[] keys = new string[] { "@input" };
            string[] values = new string[] { input };
            //perform update upon valid query else display the validity details. 
            try
            {
                if (LegalString(input))
                {
                    dataSource.UpdatePrepare(update, keys, values);
                    MessageBox.Show("Update Successful.");
                }
                else
                {
                    MessageBox.Show("Invalid entry. Please enter only alphabets and numbers.");
                }
            }
            catch (Exception error)
            {
                error.StackTrace.ToString();
                MessageBox.Show(@"Unable to perform update.\nCheck the following and try again.
                                    1.You can only update IDs with existing IDs.
                                    2.Check if you are connected to database.");
            }
        }

        /// <summary>
        /// used to execute the emergency protocol which is to DROP all tables in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropButton_Click(object sender, RoutedEventArgs e)
        {
            //produce warning message prior to execution for user to confirm
            if (MessageBox.Show("This is an irreversible action to clear the entire database.\nAre you sure you want to proceed?",
                "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do nothing
            }
            else
            {
                //execute DROP table queries
                dataSource.Query("DROP TABLE Agents");
                dataSource.Query("DROP TABLE Missions");
                dataSource.Query("DROP TABLE Departments");
                dataSource.Query("DROP TABLE Jobs");
                MessageBox.Show("Protocol Executed.\nAll data erased.");
            }

        }

        /// <summary>
        /// used to fill the combo boxes with data from the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        /// <param name="c"></param>
        private void fillCombo(string query, string name, ComboBox c)
        {
            MySqlCommand cmdReader;
            MySqlDataReader myReader;
            try
            {
                cmdReader = new MySqlCommand(query, dataSource.conn);
                myReader = cmdReader.ExecuteReader();
                while (myReader.Read())
                {
                    string temp;
                    if (name != null)
                    {
                        temp = myReader.GetString(name);
                        if (!c.Items.Contains(temp))
                        {
                            c.Items.Add(temp);
                        }
                    }
                }
                myReader.Close();
            }
            catch (InvalidOperationException e) { e.StackTrace.ToString(); }
        }

        /// <summary>
        /// used to toggle display information on the retrieve section depending on the user's choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)combo1.SelectedItem).Content.ToString() == "Agents")
            {
                //clears combobox off current data and reloads data from the database
                combo2.Items.Clear();
                fillComboColumn("SELECT * FROM Agents;", combo2);
            }
            else
            {
                //clears combobox off current data and reloads data from the database
                combo2.Items.Clear();
                fillComboColumn("SELECT * FROM Missions;", combo2);
            }
        }

        /// <summary>
        /// used to toggle display information on the update section depending on the user's selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)combo3.SelectedItem).Content.ToString() == "Agents")
            {
                combo4.Items.Clear();
                foreach (string x in tableArray)
                {
                    combo4.Items.Add(x);
                }
                combo6.Items.Clear();
                combo6.Items.Add("Agent_ID");
                //clears combobox off current data and reloads Agent column data from the database
                combo7.Items.Clear();
                fillCombo("SELECT * FROM Agents;", "Agent_ID", combo7);
            }
            else
            {
                combo4.Items.Clear();
                foreach (string x in attributeArray)
                {
                    combo4.Items.Add(x);
                }
                combo6.Items.Clear();
                combo6.Items.Add("Mission_ID");
                //clears combobox off current data and reloads Mission column data from the database
                combo7.Items.Clear();
                fillCombo("SELECT * FROM Missions;", "Mission_ID", combo7);
            }
        }

        /// <summary>
        /// Toggles between the color red and white for the textboxes depending on valid/ invalid entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //array to hold all textboxes
            TextBox[] allTextBoxArray = { textbox_missionName, textbox_missionDescr, textbox_missionCountry, textbox_missionCity , 
                                       textbox_agentName, textbox_agentAge, textbox_agentAlias, textbox_agentSecurityClearance,
                                       textBoxRetrieveData, textbox_deleteName, textbox_update};
            //checking each element in array and toggling the textbox colors depending on valid/invalid values. 
            for (int x = 0; x < allTextBoxArray.Length; x++)
            {
                if (!LegalString(allTextBoxArray[x].Text))
                {
                    allTextBoxArray[x].Background = Brushes.Red;
                }
                else
                {
                    allTextBoxArray[x].Background = Brushes.White;
                }
            }
            //checking for age and security clerance textboxes to ensure the inputs are valid whole numbers
            //and toggling the textbox color accordingly to indicate validity
            TextBox[] numberArray = { textbox_agentAge, textbox_agentSecurityClearance };
            for (int x = 0; x < numberArray.Length; x++)
            {
                if (!isNumeric(numberArray[x].Text))
                {
                    numberArray[x].Background = Brushes.Red;
                }
                else
                {
                    numberArray[x].Background = Brushes.White; 
                }
            }       
        }

        /// <summary>
        /// toggle the combo box values for the attribute selections for secondary table depending on which table under Join Query Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)secondaryTable.SelectedItem).Content.ToString() == "Departments")
            {
                //clearing previous items prior to adding Departments items
                stCombo1.Items.Clear();
                stCombo2.Items.Clear();
                hiddenText.Content = "Dept_ID"; //invisible to user. used by program in the background to make the query.
                stCombo1.Items.Add(""); stCombo2.Items.Add("");
                foreach (string x in deptArray)
                {
                    //adding the Departments items
                    stCombo1.Items.Add(x);
                    stCombo2.Items.Add(x);
                }
            }
            else if (((ComboBoxItem)secondaryTable.SelectedItem).Content.ToString() == "Missions")
            {
                //clearing previous items prior to adding Mission items
                stCombo1.Items.Clear();
                stCombo2.Items.Clear();
                hiddenText.Content = "Mission_ID"; //invisible to user. used by program in the background to make the query.
                stCombo1.Items.Add(""); stCombo2.Items.Add("");
                foreach (string x in attributeArray)
                {
                    //adding the Mission items
                    stCombo1.Items.Add(x);
                    stCombo2.Items.Add(x);
                }
            }
            else
            {
                //clearing previous items prior to adding Agents itemss
                stCombo1.Items.Clear();
                stCombo2.Items.Clear();
                hiddenText.Content = "Job_ID"; //invisible to user. used by program in the background to make the query.
                stCombo1.Items.Add(""); stCombo2.Items.Add("");
                foreach (string x in jobsArray)
                {
                    //adding the Job items
                    stCombo1.Items.Add(x);
                    stCombo2.Items.Add(x);
                }
            }
        }

        /// <summary>
        /// fill the combo boxes under the join query data with the column details of the table.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="c"></param>
        private void fillComboColumn(string query, ComboBox c)
        {
            MySqlCommand cmdReader;
            MySqlDataReader myReader;
            try
            {
                cmdReader = new MySqlCommand(query, dataSource.conn);
                myReader = cmdReader.ExecuteReader();
                for (int index = 0; index < myReader.FieldCount; index++)
                {
                    c.Items.Add(myReader.GetName(index));
                }
                myReader.Close();
                c.Items.Add("");
            }
            catch (InvalidOperationException e)
            {
                e.StackTrace.ToString();
                Console.WriteLine("Unable to load data from database");
            }
        }

        /// <summary>
        /// Used to check if the user input is of legal value
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool LegalString(string s)
        {
            string dict = "abcdefghijklmnopqrstuvwxyz0123456789. ";
            for (int i = 0; i < s.Length; i++)
            {
                if (!dict.Contains(s.Substring(i, 1).ToLower()))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// A helper method which checks whether a given string
        /// contains only numbers and is thus safe to parse to a double.
        /// </summary>
        /// <param name="toCheck">The string to check</param>
        /// <returns>True if string can be safely parsed to double</returns> 
        private bool isNumeric(string toCheck)
        {
            int outVal;
            return int.TryParse(toCheck, out outVal);  
        }
    }
}
