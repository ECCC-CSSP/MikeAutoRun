using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.Objects;
using Microsoft.VisualBasic;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using MikeAutoRun.Resources;

namespace MikeAutoRun
{
    public partial class MikeAutoRunMzLaunchFileName : Form
    {
        CSSPAppDB2Entities CSSPEntities = new CSSPAppDB2Entities();
        MikeScenario mikeScenarioToRun = null;

        private enum PurposeType
        {
            Input = 0,
            InputPol = 1,
            MikeResult = 2,
            KMZResult = 3,
            Original = 4,
            MikeScenarioOther = 5,
            MunicipalityOther = 6
        }
        public enum ScenarioStatusType
        {
            Created = 0,    // scenario has just been created
            ReadyToRun = 1, // scenario is ready to be run
            Running = 2,    // scenario being run
            Completed = 3,  // scenario ran without error
            Error = 4,      // scenario ran but has error
            Canceled = 5,     // scenario was cancelled
            Changed = 6       // scenario was saved
        }

        // Constructor
        public MikeAutoRunMzLaunchFileName()
        {
            InitializeComponent();
            timerMikeAutoRun.Enabled = false;
            timerMikeAutoRun.Interval = 5000; // 5 seconds
            timerMikeAutoRun.Start();
            CSSPEntities = new CSSPAppDB2Entities();
            richTextBoxStatus.Clear();
            dataGridViewScenarios.ColumnAdded += dataGridViewScenarios_ColumnAdded;

        }

        // private functions
        private void CheckForMoreReadyToRun()
        {
            string ReadyToRunStatus = ScenarioStatusType.ReadyToRun.ToString();
            string RunningStatus = ScenarioStatusType.Running.ToString();

            dataGridViewScenarios.DataSource = null;
            dataGridViewScenarios.DataSource = (from ms in CSSPEntities.MikeScenarios
                                                where ms.ScenarioStatus == ReadyToRunStatus
                                                || ms.ScenarioStatus == RunningStatus
                                                orderby ms.ScenarioStatus descending, ms.MikeScenarioID
                                                select ms).ToList<MikeScenario>();

            int mikeScenarioRunningCount = (from ms in CSSPEntities.MikeScenarios
                                            where ms.ScenarioStatus == RunningStatus
                                            orderby ms.MikeScenarioID
                                            select ms).Count();

            if (mikeScenarioRunningCount < 3)
            {
                mikeScenarioToRun = (from ms in CSSPEntities.MikeScenarios
                                     where ms.ScenarioStatus == ReadyToRunStatus
                                     orderby ms.MikeScenarioID
                                     select ms).FirstOrDefault<MikeScenario>();

                if (mikeScenarioToRun != null)
                {
                    AppTask appTask = (from c in CSSPEntities.AppTasks
                                       join ci in CSSPEntities.CSSPItems on c.CSSPItemID equals ci.CSSPItemID
                                       join ms in CSSPEntities.MikeScenarios on c.CSSPItemID equals ms.CSSPItemID
                                       where ms.MikeScenarioID == mikeScenarioToRun.MikeScenarioID
                                       orderby c.AppTaskID descending
                                       select c).FirstOrDefault<AppTask>();

                    if (appTask == null)
                    {
                        richTextBoxStatus.AppendText("Could not find AppTask for MikeScenarioID [" + mikeScenarioToRun.MikeScenarioID + "]\r\n");
                        return;
                    }
                    else
                    {
                        mikeScenarioToRun.ScenarioStatus = ScenarioStatusType.Running.ToString();

                        try
                        {
                                CSSPEntities.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            richTextBoxStatus.AppendText("Error while updating MikeScenario ScenarioStatus to Running ... Error Message: [" + ex.Message + "]\r\n");
                            return;
                        }

                        if (appTask.AppTaskCultureName == null)
                        {
                            appTask.AppTaskCultureName = "en-CA";
                        }
                        if (appTask.AppTaskCultureName.Length != 5)
                        {
                            appTask.AppTaskCultureName = "en-CA";
                        }
                        if (appTask.AppTaskCultureName.Substring(0, 2).ToLower() == "fr")
                        {
                            UpdateTask(appTask.AppTaskID, string.Format(MikeAutoRunRes.Running_fr));
                        }
                        else
                        {
                            UpdateTask(appTask.AppTaskID, string.Format(MikeAutoRunRes.Running));
                        }

                        try
                        {
                            CSSPEntities.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            richTextBoxStatus.AppendText("Error while updating AppTask ... Error Message: [" + ex.Message + "]\r\n");
                        }

                        richTextBoxStatus.AppendText("Trying to run MikeScenarioID = [" + mikeScenarioToRun.MikeScenarioID + "] ...\r\n");
                        if (RunScenario(mikeScenarioToRun, appTask))
                        {
                            richTextBoxStatus.AppendText("Running MikeScenarioID = [" + mikeScenarioToRun.MikeScenarioID + "] ...\r\n");
                        }
                        else
                        {
                            richTextBoxStatus.AppendText("Error while trying to run MikeScenarioID = [" + mikeScenarioToRun.MikeScenarioID + "] ...\r\n");
                        }
                    }
                }
            }

        }
        private bool IsProcessRunning(int MikeScenarioID)
        {
            StringBuilder sb = new StringBuilder();
            bool Found = false;
            Process[] processList = Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (p.MainWindowTitle.StartsWith("Mike Scenario Updater " + MikeScenarioID))
                {
                    Found = true;
                    richTextBoxStatus.AppendText("Mike Scenario Updater " + MikeScenarioID + " is already started");
                    break;
                }
                else
                {
                    Found = false;
                }
            }
            return Found;
        }
        private bool RunScenario(MikeScenario mikeScenario, AppTask appTask)
        {
            m21_3fm m21_3fmInput = new m21_3fm();
            string CurrentFileName = "";
            string HydroFileName = "";
            string TransFileName = "";

            richTextBoxStatus.AppendText("Finding m21fm or m3fm for ... " + mikeScenario.ScenarioName + "\r\n");

            CSSPFile csspFilem21_3fm = (from c in CSSPEntities.CSSPFiles
                                        join msf in CSSPEntities.MikeScenarioFiles on c.CSSPFileID equals msf.CSSPFileID
                                        where msf.MikeScenarioID == mikeScenario.MikeScenarioID
                                        && (c.FileType == ".m21fm" || c.FileType == ".m3fm")
                                        select c).FirstOrDefault<CSSPFile>();

            if (csspFilem21_3fm == null)
            {
                richTextBoxStatus.AppendText("Error: Could not find CSSPFile of type .m21fm or .m3fm and MikeScenarioID [" + mikeScenario.MikeScenarioID + "]\r\n");
                return false;
            }

            CurrentFileName = csspFilem21_3fm.ServerFilePath + csspFilem21_3fm.ServerFileName;

            FileInfo fiServer = new FileInfo(csspFilem21_3fm.ServerFilePath + csspFilem21_3fm.ServerFileName);
            if (!fiServer.Exists)
            {
                richTextBoxStatus.AppendText("File [" + fiServer.FullName + "] could not be found on the server.\r\n");
                return false;
            }

            if (!m21_3fmInput.Read_M21_3FM_File(csspFilem21_3fm.ServerFilePath + csspFilem21_3fm.ServerFileName))
            {
                richTextBoxStatus.AppendText("File [" + fiServer.FullName + "] could not be parsed properly\r\n");
                return false;
            }

            // changing the MikeScenarios DB
            // mikeScenarioToChange.ScenarioName = mikeScenario.ScenarioName;

            string DfsuFileName = fiServer.FullName;

            string FirstPart = DfsuFileName.Substring(0, DfsuFileName.LastIndexOf("\\"));
            string SecondPart = m21_3fmInput.system.ResultRootFolder.Replace("|", "\\");
            string ThirdPart = DfsuFileName.Substring(DfsuFileName.LastIndexOf("\\") + 1) + " - Result Files\\";


            // try hydrodynamic module result file
            string ForthPart = m21_3fmInput.femEngineHD.hydrodynamic_module.outputs.output.First().Value.file_name.Replace("'", "");

            FileInfo fiHydro = new FileInfo(FirstPart + SecondPart + ThirdPart + ForthPart);

            if (fiHydro.Exists)
            {
                HydroFileName = fiHydro.FullName;
            }

            // try transport module result file
            ForthPart = m21_3fmInput.femEngineHD.transport_module.outputs.output.First().Value.file_name.Replace("'", "");

            FileInfo fiTrans = new FileInfo(FirstPart + SecondPart + ThirdPart + ForthPart);

            if (fiTrans.Exists)
            {
                TransFileName = fiTrans.FullName;
            }

            richTextBoxStatus.AppendText("Preparing processMIKE to be run ...\r\n");

            if (appTask.AppTaskCultureName.Substring(0, 2).ToLower() == "fr")
            {
                UpdateTask(appTask.AppTaskID, string.Format(MikeAutoRunRes.Running_fr));
            }
            else
            {
                UpdateTask(appTask.AppTaskID, string.Format(MikeAutoRunRes.Running));
            }

            if (!IsProcessRunning(mikeScenario.MikeScenarioID))
            {
                // starting the Updater.exe application
                ProcessStartInfo pInfoUpdater = new ProcessStartInfo();
                pInfoUpdater.Arguments = " \"" + appTask.AppTaskID + "\" "
                    + " \"" + mikeScenario.MikeScenarioID + "\" "
                    + " \"" + CurrentFileName + "\" "
                    + " \"" + HydroFileName + "\" "
                    + " \"" + TransFileName + "\" "
                    + " \"" + (long)mikeScenario.EstimatedHydroFileSize + "\" "
                    + " \"" + (long)mikeScenario.EstimatedTransFileSize + "\" "
                    + " \"" + appTask.AppTaskCultureName + "\" "
                    + " \"" + mikeScenario.ModifiedByID.ToString() + "\" ";
                pInfoUpdater.WindowStyle = ProcessWindowStyle.Minimized;
                pInfoUpdater.UseShellExecute = true;

                Process processUpdater = new Process();
                processUpdater.StartInfo = pInfoUpdater;
                try
                {
                    pInfoUpdater.FileName = @"C:\CSSP_Execs\Updater\Updater.exe";
                    processUpdater.Start();
                }
                catch (Exception ex)
                {
                    UpdateTask(appTask.AppTaskID, "");
                    richTextBoxStatus.AppendText("File [" + pInfoUpdater.FileName + "] could not run. Error Message: [" + ex.Message + "]\r\n");
                    return false;
                }

                processUpdater.WaitForInputIdle(2000);
            }

            return true;
        }
        public void UpdateTask(int AppTaskID, string AppTaskStatus)
        {
            AppTask AppTaskToUpdate = (from c in CSSPEntities.AppTasks
                                       where c.AppTaskID == AppTaskID
                                       select c).FirstOrDefault<AppTask>();

            if (AppTaskToUpdate == null)
            {
                return;
            }

            AppTaskToUpdate.AppTaskStatus = AppTaskStatus;
            if (AppTaskToUpdate.AppTaskStatus == "")
            {
                CSSPEntities.AppTasks.Remove(AppTaskToUpdate);
            }

            try
            {
                CSSPEntities.SaveChanges();
            }
            catch (Exception)
            {
                // nothing for now
            }
        }

        // Events
        private void MikeAutoRun_Load(object sender, EventArgs e)
        {
            richTextBoxStatus.Clear();
            timerMikeAutoRun.Enabled = true;
        }
        private void dataGridViewScenarios_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            switch (e.Column.DataPropertyName)
            {
                case "MikeScenarioID":
                    e.Column.HeaderText = "ID";
                    break;
                case "ScenarioStatus":
                    e.Column.HeaderText = "Status";
                    break;
                case "ScenarioName":
                    e.Column.HeaderText = "Name";
                    e.Column.Width = 300;
                    break;
                case "EstimatedHydroFileSize":
                    e.Column.HeaderText = "Hydro size";
                    break;
                case "EstimatedTransFileSize":
                    e.Column.HeaderText = "Trans size";
                    break;
                case "ScenarioStartDateAndTime":
                case "ScenarioEndDateAndTime":
                case "ScenarioStartExecutionDateAndTime":
                case "ScenarioExecutionTimeInMinutes":
                case "UseWebTide":
                case "CSSPItemID":
                case "LastModifiedDate":
                case "ModifiedByID":
                case "IsActive":
                case "CSSPItem":
                case "MikeScenarioFiles":
                case "NumberOfElements":
                case "NumberOfTimeSteps":
                case "NumberOfSigmaLayers":
                case "NumberOfZLayers":
                case "NumberOfHydroOutputParameters":
                case "NumberOfTransOutputParameters":
                    e.Column.Visible = false;
                    break;
                default:
                    break;
            }

        }
        private void timerMikeAutoRun_Tick(object sender, EventArgs e)
        {
            timerMikeAutoRun.Stop();
            CheckForMoreReadyToRun();
            timerMikeAutoRun.Start();
        }
    }
}
