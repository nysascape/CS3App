﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace CSCourseworkApp
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            // Populate the group list on startup
            // and set the Data source of the group
            // list to it.
            InitializeComponent();
            RefreshLists();
            groupsListBox.DataSource = Groups.GroupList;
            staffListBox.DataSource = Staff.StaffList;
            subjectsListBox.DataSource = Subjects.SubjectList;
            studentsListBox.DataSource = Students.StudentList;
        }

        private void HidePanels(System.Windows.Forms.Panel panelToShow)
        {
            // Hide all panels except admin selection panel and desired form passed as an arugment
            foreach (Panel panel in Controls.OfType<Panel>().Where(p => p != panelToShow && p != adminPanel))
            {
                panel.Hide();
            }
            panelToShow.Show();
        }

        private void ManageGroupsButton_Click(object sender, EventArgs e)
        {
            // Show Group panel
            HidePanels(manageGroupsPanel);
        }

        private void AdminStaffButton_Click(object sender, EventArgs e)
        {
            // Show Staff panel
            HidePanels(manageStaffPanel);
        }

        private void AdminStudentsButton_Click(object sender, EventArgs e)
        {
            // Show Students panel
            HidePanels(manageStudentsPanel);
        }

        private void manageSubjectsButton_Click(object sender, EventArgs e)
        {
            // Show Subjects panel
            HidePanels(manageSubjectsPanel);
        }

        private void GroupsListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (groupsListBox.SelectedIndex != -1)
            {
                string groupName = groupsListBox.SelectedItem.ToString();
                int groupIndex = groupsListBox.SelectedIndex;
                // Make sure there isn't any index selected. This will panic otherwise.
                selectedGroupLabel.Text = Groups.GroupList[groupIndex];
                academicYearLabel.Text = $"Academic Year: {Groups.GetAcademicYear(groupName)}";
                // Seperate each lecturer by a line break, \r\n and join each staff.
                staffListLabel.Text = $"Assigned lecturer(s): {string.Join("\r\n", Groups.GetStaff(groupName))}";
                SubjectNameLabel.Text = $"Subject: {Subjects.GetSubjectName(groupName)}";
                // These buttons are hidden by default.
                editClassButton.Show();
                deleteClassButton.Show();
            }
        }

        private void EditClassButton_Click(object sender, EventArgs e)
        {
            // Deploy an edit form with the needed data.
            EditGroupForm editGroupForm = new EditGroupForm(Groups.GroupList[groupsListBox.SelectedIndex], groupsListBox.SelectedIndex + 1);
            editGroupForm.ShowDialog();
            editGroupForm.Dispose();
            // Simulate a value change to refresh changed data.
            GroupsListBox_SelectedValueChanged(this, e);
        }

        private void AddClassButton_Click(object sender, EventArgs e)
        {
            // Deploy an empty group edit form.
            EditGroupForm editGroupForm = new EditGroupForm();
            editGroupForm.ShowDialog();
            editGroupForm.Dispose();
            // Simulate a value change to refresh changed data.
            GroupsListBox_SelectedValueChanged(this, e);
        }

        private void deleteClassButton_Click(object sender, EventArgs e)
        {
            // Create a simple MessageBox for admin confirmation.
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the selected group?",
                "Delete group",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    Groups.DeleteGroup(groupsListBox.SelectedItem.ToString());
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void editStaffInfoButton_Click(object sender, EventArgs e)
        {
            EditStaffForm edf = new EditStaffForm(staffListBox.SelectedItem.ToString());
            edf.ShowDialog();
            edf.Dispose();
            staffListBox_SelectedValueChanged(this, e);
        }

        private void staffListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (staffListBox.SelectedIndex != -1)
            {

            }
        }

        private void addStaffMemberButton_Click(object sender, EventArgs e)
        {
            EditStaffForm edf = new EditStaffForm();
            edf.ShowDialog();
            edf.Dispose();
            staffListBox_SelectedValueChanged(this, e);
        }

        private void staffListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (staffListBox.SelectedIndex != -1)
            {
                staffNameLabel.Text = staffListBox.SelectedItem.ToString();
            }
        }

        private void deleteStaffButton_Click(object sender, EventArgs e)
        {
            // Create a simple MessageBox for admin confirmation.
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the selected staff member?",
                "Delete staff member",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    Staff.DeleteStaffMember(staffListBox.SelectedItem.ToString());
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private void addNewSubjectButton_Click(object sender, EventArgs e)
        {
            if (subjectNewNameBox.Text.Length != 0)
            {
                Subjects.AddNewSubject(subjectNewNameBox.Text);
            }
        }

        private void deleteSubjectButton_Click(object sender, EventArgs e)
        {
            string selectedGroup = (string)subjectsListBox.SelectedItem;
            SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM Groups WHERE SubjectId = @SubjectId");
            comm.Parameters.AddWithValue("@SubjectId", Subjects.GetSubjectIdByName(selectedGroup));
            int groupsAffected = SqlTools.ExecuteScalar(comm);
            // Create a simple MessageBox for admin confirmation.
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the selected subject?\r\n" +
                groupsAffected + " groups will be deleted!",
                "Delete subject",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            switch (dialogResult)
            {
                case DialogResult.Yes:
                    Subjects.DeleteSubject(selectedGroup);
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        public static void RefreshLists()
        {
            /*
             * Refreshes all the available lists.
             * Should be called after any database transaction
             * modifying groups, staff or subject. They are closely
             * intertwined.
             */
            Groups.PopulateList();
            Staff.PopulateList();
            Subjects.PopulateList();
            Students.PopulateList();
        }

        private void addStudentButton_Click(object sender, EventArgs e)
        {
            EditStudentForm edf = new EditStudentForm();
            edf.ShowDialog();
            edf.Dispose();
        }

        private void deleteStudentButton_Click(object sender, EventArgs e)
        {
            Students.DeleteStudent(studentsListBox.SelectedItem.ToString());
        }

        private void editStudentButton_Click(object sender, EventArgs e)
        {
            if (studentsListBox.SelectedIndex != -1)
            {
                EditStudentForm edf = new EditStudentForm(studentsListBox.SelectedItem.ToString());
                edf.ShowDialog();
                edf.Dispose();
            }
        }

        private void studentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (studentsListBox.SelectedIndex != -1)
            {
                selectedStudentLabel.Text = $"{studentsListBox.SelectedItem}";
                stuAcademicYearLabel.Text = $"Academic Year: {Students.GetAcademicYear(studentsListBox.SelectedItem.ToString())}";
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void editPrevResultsButton_Click(object sender, EventArgs e)
        {
            if (subjectsListBox.SelectedIndex != -1)
            {
                AddPreviousResultsForm aprf = new AddPreviousResultsForm
                {
                    subjectId = subjectsListBox.SelectedIndex + 1
                };
                aprf.ShowDialog();
                aprf.Dispose();
            }
        }
    }
}
