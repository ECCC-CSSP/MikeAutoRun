using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MikeAutoRun
{
    public partial class m21_3fm
    {
        // --------------------
        // m21_3fm read section
        // --------------------

        private char[] delimiter = ",".ToCharArray();
        private int LineNumber = 0;

        private string GetTheLine(StreamReader sr)
        {
            string TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            return TheLine.Trim();
        }
        private bool CheckNextLine(StreamReader sr, string TextToVerify)
        {
            string TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            TheLine = TheLine.Trim();
            if (TheLine.Length == 0 && TextToVerify.Length == 0)
            {
                return true;
            }
            else if (TheLine.Length != 0 && TextToVerify.Length == 0)
            {
                RaiseMessageEvent("\r\n Error: [empty] not found");
                return false;
            }
            else if (!TheLine.StartsWith(TextToVerify))
            {
                RaiseMessageEvent("\r\n Error: " + TextToVerify + " not found");
                return false;
            }
            return true;
        }
        private bool Read_m21_3fm_Document(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            sbFileTxt = new StringBuilder();

            RaiseMessageEvent("");
            Read_m21_3fm_TopFileInfo(sr, femEngineHD);

            if (!CheckNextLine(sr, "[FemEngineHD]")) return false;

            RaiseMessageEvent("");
            femEngineHD.domain = new m21_3fm.FemEngineHD.DOMAIN();
            if (!Read_m21_3fm_FemEngineHD_DOMAIN(sr, femEngineHD)) return false;
            RaiseMessageEvent("");
            femEngineHD.time = new m21_3fm.FemEngineHD.TIME();
            if (!Read_m21_3fm_FemEngineHD_TIME(sr, femEngineHD)) return false;
            RaiseMessageEvent("");
            femEngineHD.module_selection = new m21_3fm.FemEngineHD.MODULE_SELECTION();
            if (!Read_m21_3fm_FemEngineHD_MODULE_SELECTION(sr, femEngineHD)) return false;
            RaiseMessageEvent("");
            femEngineHD.hydrodynamic_module = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE();
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_ECOLAB_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_MUD_TRANSPORT_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_MUD_SAND_TRANSPORT_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_MUD_PARTICLE_TRACKING_MODULE(sr, femEngineHD)) return false;

            if (!CheckNextLine(sr, @"EndSect  // FemEngineHD")) return false;
            if (!CheckNextLine(sr, @"")) return false;

            Read_m21_3fm_system(sr, femEngineHD);
            return true;
        }
        private bool Read_m21_3fm_TopFileInfo(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            this.topfileinfo = new m21_3fm.TopFileInfo();
            while (!TheLine.StartsWith(@"// PFS version"))
            {
                TheLine = GetTheLine(sr);
                VariableName = TheLine.Substring(0, TheLine.IndexOf(":") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf(":") + 1).Trim();
                switch (VariableName)
                {
                    case @"// Created":
                        this.topfileinfo.Created = DateTime.Parse(TheValue);
                        break;
                    case @"// DLL id":
                        this.topfileinfo.DLLid = TheValue;
                        break;
                    case @"// PFS version":
                        this.topfileinfo.PFSversion = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_system(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SYSTEM]")) return false;
            this.system = new m21_3fm.SYSTEM();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SYSTEM") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "ResultRootFolder":
                        this.system.ResultRootFolder = TheValue;
                        break;
                    case "UseCustomResultFolder":
                        this.system.UseCustomResultFolder = bool.Parse(TheValue);
                        break;
                    case "CustomResultFolder":
                        this.system.CustomResultFolder = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_MUD_PARTICLE_TRACKING_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            StringBuilder sb = new StringBuilder();
            TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            if (TheLine.Trim() != "[PARTICLE_TRACKING_MODULE]")
            {
                RaiseMessageEvent("Error reading [PARTICLE_TRACKING_MODULE]\r\n");
                return false;
            }
            sb.AppendLine(TheLine);
            while (!(TheLine.Contains("EndSect") && TheLine.Contains("PARTICLE_TRACKING_MODULE")))
            {
                TheLine = sr.ReadLine();
                LineNumber += 1;
                this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
                sb.AppendLine(TheLine);
            }
            femEngineHD.particle_tracking_module = sb.ToString();
            if (!CheckNextLine(sr, @"")) return false;
            this.sbFileTxt.Append(femEngineHD.particle_tracking_module);
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_MUD_SAND_TRANSPORT_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            StringBuilder sb = new StringBuilder();
            TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            if (TheLine.Trim() != "[SAND_TRANSPORT_MODULE]")
            {
                RaiseMessageEvent("Error reading [SAND_TRANSPORT_MODULE]\r\n");
                return false;
            }
            sb.AppendLine(TheLine);
            while (!(TheLine.Contains("EndSect") && TheLine.Contains("SAND_TRANSPORT_MODULE")))
            {
                TheLine = sr.ReadLine();
                LineNumber += 1;
                this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
                sb.AppendLine(TheLine);
            }
            femEngineHD.sand_transport_module = sb.ToString();
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_MUD_TRANSPORT_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            StringBuilder sb = new StringBuilder();
            TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            if (TheLine.Trim() != "[MUD_TRANSPORT_MODULE]")
            {
                RaiseMessageEvent("Error reading [MUD_TRANSPORT_MODULE]\r\n");
                return false;
            }
            sb.AppendLine(TheLine);
            while (!(TheLine.Contains("EndSect") && TheLine.Contains("MUD_TRANSPORT_MODULE")))
            {
                TheLine = sr.ReadLine();
                LineNumber += 1;
                this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
                sb.AppendLine(TheLine);
            }
            femEngineHD.mud_transport_module = sb.ToString();
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_ECOLAB_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            StringBuilder sb = new StringBuilder();
            TheLine = sr.ReadLine();
            LineNumber += 1;
            this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
            if (TheLine.Trim() != "[ECOLAB_MODULE]")
            {
                RaiseMessageEvent("Error reading [ECOLAB_MODULE]\r\n");
                return false;
            }
            sb.AppendLine(TheLine);
            while (!(TheLine.Contains("EndSect") && TheLine.Contains("ECOLAB_MODULE")))
            {
                TheLine = sr.ReadLine();
                LineNumber += 1;
                this.sbFileTxt.Append(string.Format("{0}\r\n", TheLine));
                sb.AppendLine(TheLine);
            }
            femEngineHD.ecolab_module = sb.ToString();
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[TRANSPORT_MODULE]")) return false;
            femEngineHD.transport_module = new m21_3fm.FemEngineHD.TRANSPORT_MODULE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[EQUATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "mode":
                        femEngineHD.transport_module.mode = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_EQUATION(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_TIME(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SPACE_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_COMPONENTS_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOLUTION_TECHNIQUE_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_HYDRODYNAMIC_CONDITIONS_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DECAY(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_INITIAL_CONDITIONS_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // TRANSPORT_MODULE")) return false;
            if (!CheckNextLine(sr, @"")) return false;

            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[OUTPUTS]")) return false;
            femEngineHD.transport_module.outputs = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[OUTPUT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.outputs.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.outputs.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "number_of_outputs":
                        femEngineHD.transport_module.outputs.number_of_outputs = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // OUTPUTS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string OutputStr = "";

            femEngineHD.transport_module.outputs.output = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT>();
            if (!TheLine.StartsWith(@"[OUTPUT_")) return false;
            for (int i = 0; i < femEngineHD.transport_module.outputs.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                OutputStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT();
                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine.StartsWith(@"[POINT_")) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            output_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "include":
                            output_TRM.include = int.Parse(TheValue);
                            break;
                        case "title":
                            output_TRM.title = TheValue;
                            break;
                        case "file_name":
                            output_TRM.file_name = TheValue;
                            break;
                        case "type":
                            output_TRM.type = int.Parse(TheValue);
                            break;
                        case "format":
                            output_TRM.format = int.Parse(TheValue);
                            break;
                        case "flood_and_dry":
                            output_TRM.flood_and_dry = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            output_TRM.coordinate_type = TheValue;
                            break;
                        case "zone":
                            output_TRM.zone = int.Parse(TheValue);
                            break;
                        case "input_file_name":
                            output_TRM.input_file_name = TheValue;
                            break;
                        case "input_format":
                            output_TRM.input_format = int.Parse(TheValue);
                            break;
                        case "interpolation_type":
                            output_TRM.interpolation_type = int.Parse(TheValue);
                            break;
                        case "first_time_step":
                            output_TRM.first_time_step = int.Parse(TheValue);
                            break;
                        case "last_time_step":
                            output_TRM.last_time_step = int.Parse(TheValue);
                            break;
                        case "time_step_frequency":
                            output_TRM.time_step_frequency = int.Parse(TheValue);
                            break;
                        case "number_of_points":
                            output_TRM.number_of_points = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_POINT_OUT_TRM(sr, femEngineHD, output_TRM, TheLine)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_LINE_OUT_TRM(sr, femEngineHD, output_TRM)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_AREA_OUT_TRM(sr, femEngineHD, output_TRM)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_PARAMETERS_2D(sr, femEngineHD, output_TRM)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_PARAMETERS_3D(sr, femEngineHD, output_TRM)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_DISCHARGE(sr, femEngineHD, output_TRM)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_MASSBUDGET(sr, femEngineHD, output_TRM)) return false;

                femEngineHD.transport_module.outputs.output.Add(OutputStr, output_TRM);
                if (!CheckNextLine(sr, "EndSect  // " + OutputStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_MASSBUDGET(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[MASSBUDGET]")) return false;
            output_TRM.massbudget = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.MASSBUDGET();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // MASSBUDGET") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_TRM.massbudget.Touched = int.Parse(TheValue);
                        break;
                    case "COMPONENT_1":
                        output_TRM.massbudget.COMPONENT_1 = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_DISCHARGE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[DISCHARGE]")) return false;
            output_TRM.discharge = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.DISCHARGE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DISCHARGE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_TRM.discharge.Touched = int.Parse(TheValue);
                        break;
                    case "COMPONENT_1":
                        output_TRM.discharge.COMPONENT_1 = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_PARAMETERS_3D(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[PARAMETERS_3D]")) return false;
            output_TRM.parameters_3d = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.PARAMETERS_3D();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PARAMETERS_3D") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_TRM.parameters_3d.Touched = int.Parse(TheValue);
                        break;
                    case "COMPONENT_1":
                        output_TRM.parameters_3d.COMPONENT_1 = int.Parse(TheValue);
                        break;
                    case "U_VELOCITY":
                        output_TRM.parameters_3d.U_VELOCITY = int.Parse(TheValue);
                        break;
                    case "V_VELOCITY":
                        output_TRM.parameters_3d.V_VELOCITY = int.Parse(TheValue);
                        break;
                    case "W_VELOCITY":
                        output_TRM.parameters_3d.W_VELOCITY = int.Parse(TheValue);
                        break;
                    case "CFL_NUMBER":
                        output_TRM.parameters_3d.CFL_NUMBER = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_PARAMETERS_2D(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[PARAMETERS_2D]")) return false;
            output_TRM.parameters_2d = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.PARAMETERS_2D();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PARAMETERS_2D") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_TRM.parameters_2d.Touched = int.Parse(TheValue);
                        break;
                    case "COMPONENT_1":
                        output_TRM.parameters_2d.COMPONENT_1 = int.Parse(TheValue);
                        break;
                    case "U_VELOCITY":
                        output_TRM.parameters_2d.U_VELOCITY = int.Parse(TheValue);
                        break;
                    case "V_VELOCITY":
                        output_TRM.parameters_2d.V_VELOCITY = int.Parse(TheValue);
                        break;
                    case "CFL_NUMBER":
                        output_TRM.parameters_2d.CFL_NUMBER = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_AREA_OUT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[AREA]")) return false;
            output_TRM.area = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.AREA();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[POINT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_points":
                        output_TRM.area.number_of_points = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_AREA_OUT_TRM_POINT_AREA_OUT_TRM(sr, femEngineHD, output_TRM.area, TheLine)) return false;

            VariableName = "";
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // AREA") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "layer_min":
                        output_TRM.area.layer_min = int.Parse(TheValue);
                        break;
                    case "layer_max":
                        output_TRM.area.layer_max = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }

            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_AREA_OUT_TRM_POINT_AREA_OUT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.AREA area_OUT_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string PointStr = "";

            area_OUT_TRM.point = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.AREA.POINT>();
            if (!TheLine.StartsWith(@"[POINT_")) return false;
            for (int i = 0; i < area_OUT_TRM.number_of_points; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                PointStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.AREA.POINT point_AREA_OUT_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.AREA.POINT();
                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + PointStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "x":
                            point_AREA_OUT_TRM.x = double.Parse(TheValue);
                            break;
                        case "y":
                            point_AREA_OUT_TRM.y = double.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                area_OUT_TRM.point.Add(PointStr, point_AREA_OUT_TRM);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_LINE_OUT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[LINE]")) return false;
            output_TRM.line = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.LINE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // LINE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "npoints":
                        output_TRM.line.npoints = int.Parse(TheValue);
                        break;
                    case "x_first":
                        output_TRM.line.x_first = double.Parse(TheValue);
                        break;
                    case "y_first":
                        output_TRM.line.y_first = double.Parse(TheValue);
                        break;
                    case "z_first":
                        output_TRM.line.z_first = double.Parse(TheValue);
                        break;
                    case "x_last":
                        output_TRM.line.x_last = double.Parse(TheValue);
                        break;
                    case "y_last":
                        output_TRM.line.y_last = double.Parse(TheValue);
                        break;
                    case "z_last":
                        output_TRM.line.z_last = double.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_OUTPUTS_TRM_OUTPUT_TRM_POINT_OUT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT output_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string PointStr = "";

            output_TRM.point = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.POINT>();
            if (!TheLine.StartsWith(@"[POINT_")) return true;
            for (int i = 0; i < output_TRM.number_of_points; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                PointStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.POINT point_OUT_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.OUTPUTS.OUTPUT.POINT();
                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + PointStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "name":
                            point_OUT_TRM.name = TheValue;
                            break;
                        case "x":
                            point_OUT_TRM.x = double.Parse(TheValue);
                            break;
                        case "y":
                            point_OUT_TRM.y = double.Parse(TheValue);
                            break;
                        case "z":
                            point_OUT_TRM.z = double.Parse(TheValue);
                            break;
                        case "layer":
                            point_OUT_TRM.layer = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                output_TRM.point.Add(PointStr, point_OUT_TRM);
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[BOUNDARY_CONDITIONS]")) return false;
            femEngineHD.transport_module.boundary_conditions = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[CODE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.boundary_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.boundary_conditions.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM_CODE_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // BOUNDARY_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM_CODE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string CodeStr = "";

            femEngineHD.transport_module.boundary_conditions.code = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE>();
            if (!TheLine.StartsWith(@"[CODE_")) return false;
            for (int i = 0; i < femEngineHD.transport_module.boundary_conditions.MzSEPfsListItemCount + 1; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                CodeStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE code_BC_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE();
                if (CodeStr == "CODE_1")
                {
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // " + CodeStr) break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            default:
                                return false;
                        }
                    }
                    if (!CheckNextLine(sr, "")) return false;
                }
                else
                {
                    VariableName = "";
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine.StartsWith(@"[COMPONENT_")) break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "Touched":
                                code_BC_TRM.Touched = int.Parse(TheValue);
                                break;
                            case "MzSEPfsListItemCount":
                                code_BC_TRM.MzSEPfsListItemCount = int.Parse(TheValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM_CODE_TRM_COMPONENT_BC_TRM(sr, femEngineHD, code_BC_TRM, TheLine)) return false;
                    if (!CheckNextLine(sr, "EndSect  // " + CodeStr)) return false;
                    if (!CheckNextLine(sr, "")) return false;
                }
                femEngineHD.transport_module.boundary_conditions.code.Add(CodeStr, code_BC_TRM);

            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_BOUNDARY_CONDITIONS_TRM_CODE_TRM_COMPONENT_BC_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE code_BC_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            code_BC_TRM.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE.COMPONENT>();
            if (!TheLine.StartsWith(@"[COMPONENT_")) return false;
            for (int i = 0; i < code_BC_TRM.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE.COMPONENT component_BC_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.BOUNDARY_CONDITIONS.CODE.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ComponentStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            component_BC_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            component_BC_TRM.type = int.Parse(TheValue);
                            break;
                        case "format":
                            component_BC_TRM.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            component_BC_TRM.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            component_BC_TRM.file_name = TheValue;
                            break;
                        case "item_number":
                            component_BC_TRM.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            component_BC_TRM.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            component_BC_TRM.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            component_BC_TRM.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            component_BC_TRM.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            component_BC_TRM.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        case "type_of_space_interpolation":
                            component_BC_TRM.type_of_space_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                code_BC_TRM.component.Add(ComponentStr, component_BC_TRM);
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_INITIAL_CONDITIONS_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[INITIAL_CONDITIONS]")) return false;
            femEngineHD.transport_module.initial_conditions = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.INITIAL_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.initial_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.initial_conditions.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        if (!CheckNextLine(sr, "[ERROR at] - Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_INITIAL_CONDITIONS_TRM" + "\r\n" + TheLine + "\r\n")) return false;
                        break;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_INITIAL_CONDITIONS_TRM_COMPONENT_INIT_COND_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // INITIAL_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_INITIAL_CONDITIONS_TRM_COMPONENT_INIT_COND_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.initial_conditions.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.INITIAL_CONDITIONS.COMPONENT>();
            if (!TheLine.StartsWith(@"[COMPONENT_")) return true;
            for (int i = 0; i < femEngineHD.transport_module.initial_conditions.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.INITIAL_CONDITIONS.COMPONENT component_INIT_COND_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.INITIAL_CONDITIONS.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ComponentStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            component_INIT_COND_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            component_INIT_COND_TRM.type = int.Parse(TheValue);
                            break;
                        case "format":
                            component_INIT_COND_TRM.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            component_INIT_COND_TRM.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            component_INIT_COND_TRM.file_name = TheValue;
                            break;
                        case "item_number":
                            component_INIT_COND_TRM.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            component_INIT_COND_TRM.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            component_INIT_COND_TRM.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            component_INIT_COND_TRM.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            component_INIT_COND_TRM.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            component_INIT_COND_TRM.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.transport_module.initial_conditions.component.Add(ComponentStr, component_INIT_COND_TRM);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[SOURCES]")) return false;
            femEngineHD.transport_module.sources = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[SOURCE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.sources.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.sources.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM_SOURCE_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // SOURCES")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM_SOURCE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string SourceStr = "";

            femEngineHD.transport_module.sources.source = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE>();
            if (!TheLine.StartsWith("[SOURCE_")) return false;
            for (int i = 0; i < femEngineHD.transport_module.sources.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                SourceStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE source_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine.StartsWith(@"[COMPONENT_")) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            source_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "MzSEPfsListItemCount":
                            source_TRM.MzSEPfsListItemCount = int.Parse(TheValue);
                            break;
                        case "name":
                            source_TRM.name = TheValue;
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM_SOURCE_TRM_COMPONENT_SOURCE_TRM(sr, femEngineHD, source_TRM, TheLine)) return false;
                femEngineHD.transport_module.sources.source.Add(SourceStr, source_TRM);

                if (!CheckNextLine(sr, "EndSect  // " + SourceStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOURCES_TRM_SOURCE_TRM_COMPONENT_SOURCE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE source_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            source_TRM.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE.COMPONENT>();
            if (!TheLine.StartsWith(@"[COMPONENT_")) return false;
            for (int i = 0; i < source_TRM.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE.COMPONENT component_SOURCE_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOURCES.SOURCE.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ComponentStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "type_of_component":
                            component_SOURCE_TRM.type_of_component = int.Parse(TheValue);
                            break;
                        case "type":
                            component_SOURCE_TRM.type = int.Parse(TheValue);
                            break;
                        case "format":
                            component_SOURCE_TRM.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            component_SOURCE_TRM.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            component_SOURCE_TRM.file_name = TheValue;
                            break;
                        case "item_number":
                            component_SOURCE_TRM.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            component_SOURCE_TRM.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            component_SOURCE_TRM.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            component_SOURCE_TRM.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            component_SOURCE_TRM.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            component_SOURCE_TRM.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                source_TRM.component.Add(ComponentStr, component_SOURCE_TRM);
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[PRECIPITATION_EVAPORATION]")) return false;
            femEngineHD.transport_module.precipitation_evaporation = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.precipitation_evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.precipitation_evaporation.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // PRECIPITATION_EVAPORATION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.precipitation_evaporation.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT>();

            for (int i = 0; i < femEngineHD.transport_module.precipitation_evaporation.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT component_PE_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"[PRECIPITATION]") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            component_PE_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "type_of_precipitation":
                            component_PE_TRM.type_of_precipitation = int.Parse(TheValue);
                            break;
                        case "type_of_evaporation":
                            component_PE_TRM.type_of_evaporation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM_PRECIPITATION(sr, femEngineHD, component_PE_TRM, TheLine)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM_EVAPORATION(sr, femEngineHD, component_PE_TRM)) return false;
                femEngineHD.transport_module.precipitation_evaporation.component.Add(ComponentStr, component_PE_TRM);

                if (!CheckNextLine(sr, "EndSect  // " + ComponentStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM_EVAPORATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT component_PE_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[EVAPORATION]")) return false;
            component_PE_TRM.evaporation = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT.EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EVAPORATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        component_PE_TRM.evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        component_PE_TRM.evaporation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        component_PE_TRM.evaporation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        component_PE_TRM.evaporation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        component_PE_TRM.evaporation.file_name = TheValue;
                        break;
                    case "item_number":
                        component_PE_TRM.evaporation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        component_PE_TRM.evaporation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        component_PE_TRM.evaporation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        component_PE_TRM.evaporation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        component_PE_TRM.evaporation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        component_PE_TRM.evaporation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_PRECIPITATION_EVAPORATION_TRM_COMPONENT_PE_TRM_PRECIPITATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT component_PE_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (TheLine != "[PRECIPITATION]") return false;
            component_PE_TRM.precipitation = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.PRECIPITATION_EVAPORATION.COMPONENT.PRECIPITATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PRECIPITATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        component_PE_TRM.precipitation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        component_PE_TRM.precipitation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        component_PE_TRM.precipitation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        component_PE_TRM.precipitation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        component_PE_TRM.precipitation.file_name = TheValue;
                        break;
                    case "item_number":
                        component_PE_TRM.precipitation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        component_PE_TRM.precipitation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        component_PE_TRM.precipitation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        component_PE_TRM.precipitation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        component_PE_TRM.precipitation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        component_PE_TRM.precipitation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DECAY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[DECAY]")) return false;
            femEngineHD.transport_module.decay = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DECAY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.decay.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.decay.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DECAY_COMPONENT_DECAY(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // DECAY")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DECAY_COMPONENT_DECAY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.decay.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DECAY.COMPONENT>();

            for (int i = 0; i < femEngineHD.transport_module.decay.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.DECAY.COMPONENT component_DECAY = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DECAY.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ComponentStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            component_DECAY.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            component_DECAY.type = int.Parse(TheValue);
                            break;
                        case "format":
                            component_DECAY.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            component_DECAY.constant_value = double.Parse(TheValue);
                            break;
                        case "file_name":
                            component_DECAY.file_name = TheValue;
                            break;
                        case "item_number":
                            component_DECAY.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            component_DECAY.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            component_DECAY.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            component_DECAY.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            component_DECAY.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            component_DECAY.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.transport_module.decay.component.Add(ComponentStr, component_DECAY);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            if (!CheckNextLine(sr, "[DISPERSION]")) return false;
            femEngineHD.transport_module.dispersion = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION();
            Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM(sr, femEngineHD);
            Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM(sr, femEngineHD);
            if (!CheckNextLine(sr, @"EndSect  // DISPERSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[VERTICAL_DISPERSION]")) return false;
            femEngineHD.transport_module.dispersion.vertical_dispersion = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.dispersion.vertical_dispersion.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.dispersion.vertical_dispersion.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // VERTICAL_DISPERSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.dispersion.vertical_dispersion.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT>();

            for (int i = 0; i < femEngineHD.transport_module.dispersion.vertical_dispersion.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT vertical_dispersion_COMPONENT_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"[SCALED_EDDY_VISCOSITY]") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            vertical_dispersion_COMPONENT_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            vertical_dispersion_COMPONENT_TRM.type = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_SCALED_EDDY_VISCOSITY_TRM(sr, femEngineHD, vertical_dispersion_COMPONENT_TRM, TheLine)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_DISPERSION_COEFFICIENT_TRM(sr, femEngineHD, vertical_dispersion_COMPONENT_TRM)) return false;

                femEngineHD.transport_module.dispersion.vertical_dispersion.component.Add(ComponentStr, vertical_dispersion_COMPONENT_TRM);

                if (!CheckNextLine(sr, "EndSect  // " + ComponentStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_SCALED_EDDY_VISCOSITY_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT vertical_dispersion_COMPONENT_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (TheLine != "[SCALED_EDDY_VISCOSITY]") return false;
            vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT.SCALED_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SCALED_EDDY_VISCOSITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.format = int.Parse(TheValue);
                        break;
                    case "sigma":
                        vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.sigma = float.Parse(TheValue);
                        break;
                    case "file_name":
                        vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.file_name = TheValue;
                        break;
                    case "item_number":
                        vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        vertical_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_VERTICAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_DISPERSION_COEFFICIENT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT vertical_dispersion_COMPONENT_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[DISPERSION_COEFFICIENT]")) return false;
            vertical_dispersion_COMPONENT_TRM.dispersion_coefficient = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.VERTICAL_DISPERSION.COMPONENT.DISPERSION_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DISPERSION_COEFFICIENT") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        vertical_dispersion_COMPONENT_TRM.dispersion_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        vertical_dispersion_COMPONENT_TRM.dispersion_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        vertical_dispersion_COMPONENT_TRM.dispersion_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        vertical_dispersion_COMPONENT_TRM.dispersion_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        vertical_dispersion_COMPONENT_TRM.dispersion_coefficient.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[HORIZONTAL_DISPERSION]")) return false;
            femEngineHD.transport_module.dispersion.horizontal_dispersion = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.dispersion.horizontal_dispersion.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.dispersion.horizontal_dispersion.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // HORIZONTAL_DISPERSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.dispersion.horizontal_dispersion.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT>();

            for (int i = 0; i < femEngineHD.transport_module.dispersion.horizontal_dispersion.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT horizontal_dispersion_COMPONENT_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"[SCALED_EDDY_VISCOSITY]") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            horizontal_dispersion_COMPONENT_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            horizontal_dispersion_COMPONENT_TRM.type = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_SCALED_EDDY_VISCOSITY_TRM(sr, femEngineHD, horizontal_dispersion_COMPONENT_TRM, TheLine)) return false;
                if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_DISPERSION_COEFFICIENT_TRM(sr, femEngineHD, horizontal_dispersion_COMPONENT_TRM)) return false;

                femEngineHD.transport_module.dispersion.horizontal_dispersion.component.Add(ComponentStr, horizontal_dispersion_COMPONENT_TRM);

                if (!CheckNextLine(sr, "EndSect  // " + ComponentStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_SCALED_EDDY_VISCOSITY_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT horizontal_dispersion_COMPONENT_TRM, string TheLine)
        {
            RaiseMessageEvent("");
            //            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (TheLine != "[SCALED_EDDY_VISCOSITY]") return false;
            horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT.SCALED_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SCALED_EDDY_VISCOSITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.format = int.Parse(TheValue);
                        break;
                    case "sigma":
                        horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.sigma = float.Parse(TheValue);
                        break;
                    case "file_name":
                        horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.file_name = TheValue;
                        break;
                    case "item_number":
                        horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        horizontal_dispersion_COMPONENT_TRM.scaled_eddy_viscosity.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_DISPERSION_TRM_HORIZONTAL_DISPERSION_TRM_DISPERSION_COMPONENT_TRM_DISPERSION_COEFFICIENT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT horizontal_dispersion_COMPONENT_TRM)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[DISPERSION_COEFFICIENT]")) return false;
            horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.DISPERSION.HORIZONTAL_DISPERSION.COMPONENT.DISPERSION_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DISPERSION_COEFFICIENT") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        horizontal_dispersion_COMPONENT_TRM.dispersion_coefficient.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_HYDRODYNAMIC_CONDITIONS_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[HYDRODYNAMIC_CONDITIONS]")) return false;
            femEngineHD.transport_module.hydrodynamic_conditions = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.HYDRODYNAMIC_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // HYDRODYNAMIC_CONDITIONS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.hydrodynamic_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.transport_module.hydrodynamic_conditions.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.transport_module.hydrodynamic_conditions.format = int.Parse(TheValue);
                        break;
                    case "surface_elevation_constant":
                        femEngineHD.transport_module.hydrodynamic_conditions.surface_elevation_constant = float.Parse(TheValue);
                        break;
                    case "u_velocity_constant":
                        femEngineHD.transport_module.hydrodynamic_conditions.u_velocity_constant = float.Parse(TheValue);
                        break;
                    case "v_velocity_constant":
                        femEngineHD.transport_module.hydrodynamic_conditions.v_velocity_constant = float.Parse(TheValue);
                        break;
                    case "w_velocity_constant":
                        femEngineHD.transport_module.hydrodynamic_conditions.w_velocity_constant = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.transport_module.hydrodynamic_conditions.file_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SOLUTION_TECHNIQUE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[SOLUTION_TECHNIQUE]")) return false;
            femEngineHD.transport_module.solution_technique = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.SOLUTION_TECHNIQUE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SOLUTION_TECHNIQUE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.solution_technique.Touched = int.Parse(TheValue);
                        break;
                    case "scheme_of_time_integration":
                        femEngineHD.transport_module.solution_technique.scheme_of_time_integration = int.Parse(TheValue);
                        break;
                    case "scheme_of_space_discretization_horizontal":
                        femEngineHD.transport_module.solution_technique.scheme_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    case "scheme_of_space_discretization_vertical":
                        femEngineHD.transport_module.solution_technique.scheme_of_space_discretization_vertical = int.Parse(TheValue);
                        break;
                    case "method_of_space_discretization_horizontal":
                        femEngineHD.transport_module.solution_technique.method_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_COMPONENTS_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[COMPONENTS]")) return false;
            femEngineHD.transport_module.components = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.COMPONENTS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[COMPONENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.transport_module.components.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.transport_module.components.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "number_of_components":
                        femEngineHD.transport_module.components.number_of_components = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_COMPONENTS_TRM_COMPONENT_TRM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // COMPONENTS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_COMPONENTS_TRM_COMPONENT_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ComponentStr = "";

            femEngineHD.transport_module.components.component = new Dictionary<string, m21_3fm.FemEngineHD.TRANSPORT_MODULE.COMPONENTS.COMPONENT>();

            for (int i = 0; i < femEngineHD.transport_module.components.number_of_components; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ComponentStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.TRANSPORT_MODULE.COMPONENTS.COMPONENT component_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.COMPONENTS.COMPONENT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ComponentStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            component_TRM.Touched = int.Parse(TheValue);
                            break;
                        case "name":
                            component_TRM.name = TheValue;
                            break;
                        case "type":
                            component_TRM.type = int.Parse(TheValue);
                            break;
                        case "dimension":
                            component_TRM.dimension = int.Parse(TheValue);
                            break;
                        case "description":
                            component_TRM.description = TheValue;
                            break;
                        case "EUM_type":
                            component_TRM.EUM_type = int.Parse(TheValue);
                            break;
                        case "EUM_unit":
                            component_TRM.EUM_unit = int.Parse(TheValue);
                            break;
                        case "unit":
                            component_TRM.unit = TheValue;
                            break;
                        case "minimum_value":
                            component_TRM.minimum_value = float.Parse(TheValue);
                            break;
                        case "maximum_value":
                            component_TRM.maximum_value = double.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.transport_module.components.component.Add(ComponentStr, component_TRM);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_SPACE_TRM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[SPACE]")) return false;
            femEngineHD.transport_module.space_TRM = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.SPACE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SPACE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_2D_mesh_geometry":
                        femEngineHD.transport_module.space_TRM.number_of_2D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_2D_mesh_velocity":
                        femEngineHD.transport_module.space_TRM.number_of_2D_mesh_velocity = int.Parse(TheValue);
                        break;
                    case "number_of_2D_mesh_concentration":
                        femEngineHD.transport_module.space_TRM.number_of_2D_mesh_concentration = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_geometry":
                        femEngineHD.transport_module.space_TRM.number_of_3D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_velocity":
                        femEngineHD.transport_module.space_TRM.number_of_3D_mesh_velocity = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_concentration":
                        femEngineHD.transport_module.space_TRM.number_of_3D_mesh_concentration = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_TIME(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[TIME]")) return false;
            femEngineHD.transport_module.time = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.TIME();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TIME") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "start_time_step":
                        femEngineHD.transport_module.time.start_time_step = int.Parse(TheValue);
                        break;
                    case "time_step_factor":
                        femEngineHD.transport_module.time.time_step_factor = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TRANSPORT_MODULE_EQUATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (TheLine != "[EQUATION]") return false;
            femEngineHD.transport_module.equation = new m21_3fm.FemEngineHD.TRANSPORT_MODULE.EQUATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EQUATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "formulation":
                        femEngineHD.transport_module.equation.formulation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[HYDRODYNAMIC_MODULE]")) return false;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[EQUATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "mode":
                        femEngineHD.hydrodynamic_module.mode = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EQUATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIME(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SPACE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOLUTION_TECHNIQUE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_FLOOD_AND_DRY(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DEPTH(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DENSITY(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_CORIOLIS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_WIND_FORCING(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_ICE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIDAL_POTENTIAL(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_RADIATION_STRESS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOURCES_HD(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_CONDITIONS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BOUNDARY_CONDITIONS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DECOUPLING(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, "EndSect  // HYDRODYNAMIC_MODULE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[OUTPUTS]")) return false;
            femEngineHD.hydrodynamic_module.outputs = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[OUTPUT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.outputs.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.outputs.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "number_of_outputs":
                        femEngineHD.hydrodynamic_module.outputs.number_of_outputs = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // OUTPUTS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string OutputStr = "";

            femEngineHD.hydrodynamic_module.outputs.output = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.outputs.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                OutputStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine.StartsWith("[POINT_")) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            output_HD.Touched = int.Parse(TheValue);
                            break;
                        case "include":
                            output_HD.include = int.Parse(TheValue);
                            break;
                        case "title":
                            output_HD.title = TheValue;
                            break;
                        case "file_name":
                            output_HD.file_name = TheValue;
                            break;
                        case "type":
                            output_HD.type = int.Parse(TheValue);
                            break;
                        case "format":
                            output_HD.format = int.Parse(TheValue);
                            break;
                        case "flood_and_dry":
                            output_HD.flood_and_dry = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            output_HD.coordinate_type = TheValue;
                            break;
                        case "zone":
                            output_HD.zone = int.Parse(TheValue);
                            break;
                        case "input_file_name":
                            output_HD.input_file_name = TheValue;
                            break;
                        case "input_format":
                            output_HD.input_format = int.Parse(TheValue);
                            break;
                        case "interpolation_type":
                            output_HD.interpolation_type = int.Parse(TheValue);
                            break;
                        case "first_time_step":
                            output_HD.first_time_step = int.Parse(TheValue);
                            break;
                        case "last_time_step":
                            output_HD.last_time_step = int.Parse(TheValue);
                            break;
                        case "time_step_frequency":
                            output_HD.time_step_frequency = int.Parse(TheValue);
                            break;
                        case "number_of_points":
                            output_HD.number_of_points = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_POINT_OUT_HD(sr, femEngineHD, output_HD, TheLine)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_LINE_OUT_HD(sr, femEngineHD, output_HD)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_AREA_OUT_HD(sr, femEngineHD, output_HD)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_PARAMETERS_2D(sr, femEngineHD, output_HD)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_PARAMETERS_3D(sr, femEngineHD, output_HD)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_DISCHARGE(sr, femEngineHD, output_HD)) return false;
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_MASSBUDGET(sr, femEngineHD, output_HD)) return false;
                femEngineHD.hydrodynamic_module.outputs.output.Add(OutputStr, output_HD);

                if (!CheckNextLine(sr, @"EndSect  // " + OutputStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_MASSBUDGET(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[MASSBUDGET]")) return false;
            output_HD.massbudget = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.MASSBUDGET();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // MASSBUDGET") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_HD.massbudget.Touched = int.Parse(TheValue);
                        break;
                    case "DISCHARGE":
                        output_HD.massbudget.DISCHARGE = int.Parse(TheValue);
                        break;
                    case "ACCUMULATED_DISCHARGE":
                        output_HD.massbudget.ACCUMULATED_DISCHARGE = int.Parse(TheValue);
                        break;
                    case "FLOW":
                        output_HD.massbudget.FLOW = int.Parse(TheValue);
                        break;
                    case "TEMPERATURE":
                        output_HD.massbudget.TEMPERATURE = int.Parse(TheValue);
                        break;
                    case "SALINITY":
                        output_HD.massbudget.SALINITY = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_DISCHARGE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DISCHARGE]")) return false;
            output_HD.discharge = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.DISCHARGE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DISCHARGE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_HD.discharge.Touched = int.Parse(TheValue);
                        break;
                    case "DISCHARGE":
                        output_HD.discharge.discharge = int.Parse(TheValue);
                        break;
                    case "ACCUMULATED_DISCHARGE":
                        output_HD.discharge.ACCUMULATED_DISCHARGE = int.Parse(TheValue);
                        break;
                    case "FLOW":
                        output_HD.discharge.FLOW = int.Parse(TheValue);
                        break;
                    case "TEMPERATURE":
                        output_HD.discharge.TEMPERATURE = int.Parse(TheValue);
                        break;
                    case "SALINITY":
                        output_HD.discharge.SALINITY = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_PARAMETERS_3D(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[PARAMETERS_3D]")) return false;
            output_HD.parameters_3d = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.PARAMETERS_3D();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PARAMETERS_3D") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_HD.parameters_3d.Touched = int.Parse(TheValue);
                        break;
                    case "U_VELOCITY":
                        output_HD.parameters_3d.U_VELOCITY = int.Parse(TheValue);
                        break;
                    case "V_VELOCITY":
                        output_HD.parameters_3d.V_VELOCITY = int.Parse(TheValue);
                        break;
                    case "W_VELOCITY":
                        output_HD.parameters_3d.W_VELOCITY = int.Parse(TheValue);
                        break;
                    case "WS_VELOCITY":
                        output_HD.parameters_3d.WS_VELOCITY = int.Parse(TheValue);
                        break;
                    case "DENSITY":
                        output_HD.parameters_3d.DENSITY = int.Parse(TheValue);
                        break;
                    case "TEMPERATURE":
                        output_HD.parameters_3d.TEMPERATURE = int.Parse(TheValue);
                        break;
                    case "SALINITY":
                        output_HD.parameters_3d.SALINITY = int.Parse(TheValue);
                        break;
                    case "TURBULENT_KINETIC_ENERGY":
                        output_HD.parameters_3d.TURBULENT_KINETIC_ENERGY = int.Parse(TheValue);
                        break;
                    case "DISSIPATION_OF_TKE":
                        output_HD.parameters_3d.DISSIPATION_OF_TKE = int.Parse(TheValue);
                        break;
                    case "CURRENT_SPEED":
                        output_HD.parameters_3d.CURRENT_SPEED = int.Parse(TheValue);
                        break;
                    case "CURRENT_DIRECTION_HORIZONTAL":
                        output_HD.parameters_3d.CURRENT_DIRECTION_HORIZONTAL = int.Parse(TheValue);
                        break;
                    case "CURRENT_DIRECTION_VERTICAL":
                        output_HD.parameters_3d.CURRENT_DIRECTION_VERTICAL = int.Parse(TheValue);
                        break;
                    case "HORIZONTAL_EDDY_VISCOSITY":
                        output_HD.parameters_3d.HORIZONTAL_EDDY_VISCOSITY = int.Parse(TheValue);
                        break;
                    case "VERTICAL_EDDY_VISCOSITY":
                        output_HD.parameters_3d.VERTICAL_EDDY_VISCOSITY = int.Parse(TheValue);
                        break;
                    case "CFL_NUMBER":
                        output_HD.parameters_3d.CFL_NUMBER = int.Parse(TheValue);
                        break;
                    case "VOLUME":
                        output_HD.parameters_3d.VOLUME = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_PARAMETERS_2D(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[PARAMETERS_2D]")) return false;
            output_HD.parameters_2d = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.PARAMETERS_2D();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PARAMETERS_2D") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        output_HD.parameters_2d.Touched = int.Parse(TheValue);
                        break;
                    case "SURFACE_ELEVATION":
                        output_HD.parameters_2d.SURFACE_ELEVATION = int.Parse(TheValue);
                        break;
                    case "STILL_WATER_DEPTH":
                        output_HD.parameters_2d.STILL_WATER_DEPTH = int.Parse(TheValue);
                        break;
                    case "TOTAL_WATER_DEPTH":
                        output_HD.parameters_2d.TOTAL_WATER_DEPTH = int.Parse(TheValue);
                        break;
                    case "U_VELOCITY":
                        output_HD.parameters_2d.U_VELOCITY = int.Parse(TheValue);
                        break;
                    case "V_VELOCITY":
                        output_HD.parameters_2d.V_VELOCITY = int.Parse(TheValue);
                        break;
                    case "P_FLUX":
                        output_HD.parameters_2d.P_FLUX = int.Parse(TheValue);
                        break;
                    case "Q_FLUX":
                        output_HD.parameters_2d.Q_FLUX = int.Parse(TheValue);
                        break;
                    case "DENSITY":
                        output_HD.parameters_2d.DENSITY = int.Parse(TheValue);
                        break;
                    case "TEMPERATURE":
                        output_HD.parameters_2d.TEMPERATURE = int.Parse(TheValue);
                        break;
                    case "SALINITY":
                        output_HD.parameters_2d.SALINITY = int.Parse(TheValue);
                        break;
                    case "CURRENT_SPEED":
                        output_HD.parameters_2d.CURRENT_SPEED = int.Parse(TheValue);
                        break;
                    case "CURRENT_DIRECTION":
                        output_HD.parameters_2d.CURRENT_DIRECTION = int.Parse(TheValue);
                        break;
                    case "WIND_U_VELOCITY":
                        output_HD.parameters_2d.WIND_U_VELOCITY = int.Parse(TheValue);
                        break;
                    case "WIND_V_VELOCITY":
                        output_HD.parameters_2d.WIND_V_VELOCITY = int.Parse(TheValue);
                        break;
                    case "AIR_PRESSURE":
                        output_HD.parameters_2d.AIR_PRESSURE = int.Parse(TheValue);
                        break;
                    case "PRECIPITATION":
                        output_HD.parameters_2d.PRECIPITATION = int.Parse(TheValue);
                        break;
                    case "EVAPORATION":
                        output_HD.parameters_2d.EVAPORATION = int.Parse(TheValue);
                        break;
                    case "DRAG_COEFFICIENT":
                        output_HD.parameters_2d.DRAG_COEFFICIENT = int.Parse(TheValue);
                        break;
                    case "EDDY_VISCOSITY":
                        output_HD.parameters_2d.EDDY_VISCOSITY = int.Parse(TheValue);
                        break;
                    case "CFL_NUMBER":
                        output_HD.parameters_2d.CFL_NUMBER = int.Parse(TheValue);
                        break;
                    case "CONVERGENCE_ANGLE":
                        output_HD.parameters_2d.CONVERGENCE_ANGLE = int.Parse(TheValue);
                        break;
                    case "AREA":
                        output_HD.parameters_2d.AREA = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_AREA_OUT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[AREA]")) return false;
            output_HD.area = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.AREA();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[POINT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_points":
                        output_HD.area.number_of_points = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_AREA_OUT_HD_POINT_AREA_OUT_HD(sr, femEngineHD, output_HD.area, TheLine);
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // AREA") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "layer_min":
                        output_HD.area.layer_min = int.Parse(TheValue);
                        break;
                    case "layer_max":
                        output_HD.area.layer_max = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }

            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_AREA_OUT_HD_POINT_AREA_OUT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.AREA area_OUT_HD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string PointStr = "";

            area_OUT_HD.point = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.AREA.POINT>();

            for (int i = 0; i < area_OUT_HD.number_of_points; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                PointStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.AREA.POINT point_AREA_OUT_HD = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.AREA.POINT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + PointStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "x":
                            point_AREA_OUT_HD.x = double.Parse(TheValue);
                            break;
                        case "y":
                            point_AREA_OUT_HD.y = double.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                area_OUT_HD.point.Add(PointStr, point_AREA_OUT_HD);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_LINE_OUT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[LINE]")) return false;
            output_HD.line = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.LINE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // LINE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "npoints":
                        output_HD.line.npoints = int.Parse(TheValue);
                        break;
                    case "x_first":
                        output_HD.line.x_first = double.Parse(TheValue);
                        break;
                    case "y_first":
                        output_HD.line.y_first = double.Parse(TheValue);
                        break;
                    case "z_first":
                        output_HD.line.z_first = double.Parse(TheValue);
                        break;
                    case "x_last":
                        output_HD.line.x_last = double.Parse(TheValue);
                        break;
                    case "y_last":
                        output_HD.line.y_last = double.Parse(TheValue);
                        break;
                    case "z_last":
                        output_HD.line.z_last = double.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_OUTPUTS_HD_OUTPUT_HD_POINT_OUT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT output_HD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string OutputStr = "";

            output_HD.point = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.POINT>();

            for (int i = 0; i < output_HD.number_of_points; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                OutputStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.POINT point_OUT_HD = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.OUTPUTS.OUTPUT.POINT();

                VariableName = "";
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + OutputStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "name":
                            point_OUT_HD.name = TheValue;
                            break;
                        case "x":
                            point_OUT_HD.x = double.Parse(TheValue);
                            break;
                        case "y":
                            point_OUT_HD.y = double.Parse(TheValue);
                            break;
                        case "z":
                            point_OUT_HD.z = double.Parse(TheValue);
                            break;
                        case "layer":
                            point_OUT_HD.layer = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                output_HD.point.Add(OutputStr, point_OUT_HD);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DECOUPLING(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DECOUPLING]")) return false;
            femEngineHD.hydrodynamic_module.decoupling = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DECOUPLING();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DECOUPLING") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.decoupling.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.decoupling.type = int.Parse(TheValue);
                        break;
                    case "file_name_flux":
                        femEngineHD.hydrodynamic_module.decoupling.file_name_flux = TheValue;
                        break;
                    case "file_name_area":
                        femEngineHD.hydrodynamic_module.decoupling.file_name_area = TheValue;
                        break;
                    case "file_name_volume":
                        femEngineHD.hydrodynamic_module.decoupling.file_name_volume = TheValue;
                        break;
                    case "specification_file":
                        femEngineHD.hydrodynamic_module.decoupling.specification_file = TheValue;
                        break;
                    case "first_time_step":
                        femEngineHD.hydrodynamic_module.decoupling.first_time_step = int.Parse(TheValue);
                        break;
                    case "last_time_step":
                        femEngineHD.hydrodynamic_module.decoupling.last_time_step = int.Parse(TheValue);
                        break;
                    case "time_step_frequency":
                        femEngineHD.hydrodynamic_module.decoupling.time_step_frequency = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[TURBULENCE_MODULE]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[TIME]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "mode":
                        femEngineHD.hydrodynamic_module.turbulence_module.mode = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_TIME(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SPACE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_EQUATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOLUTION_TECHNIQUE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_DIFFUSION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOURCES_TM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_BOUNDARY_CONDITIONS_TM(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, "EndSect  // TURBULENCE_MODULE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_BOUNDARY_CONDITIONS_TM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[BOUNDARY_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[CODE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_BOUNDARY_CONDITIONS_TM_CODE_TM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // BOUNDARY_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_BOUNDARY_CONDITIONS_TM_CODE_TM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string CodeStr = "";

            femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions.code = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions.MzSEPfsListItemCount + 1; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                CodeStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE code_TM = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE();

                if (CodeStr == "CODE_1")
                {
                    if (!CheckNextLine(sr, "[KINETIC_ENERGY]")) return false;
                    code_TM.kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE.KINETIC_ENERGY();
                    if (!CheckNextLine(sr, @"EndSect  // KINETIC_ENERGY")) return false;
                    if (!CheckNextLine(sr, "")) return false;
                    if (!CheckNextLine(sr, "[DISSIPATION_OF_KINETIC_ENERGY]")) return false;
                    code_TM.dissipation_of_kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE.DISSIPATION_OF_KINETIC_ENERGY();
                    if (!CheckNextLine(sr, @"EndSect  // DISSIPATION_OF_KINETIC_ENERGY")) return false;
                    if (!CheckNextLine(sr, "")) return false;
                }
                else
                {
                    VariableName = "";
                    if (!CheckNextLine(sr, "[KINETIC_ENERGY]")) return false;
                    code_TM.kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE.KINETIC_ENERGY();
                    int count = 0;
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // KINETIC_ENERGY") break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "identifier":
                                code_TM.kinetic_energy.identifier = TheValue;
                                break;
                            case "type":
                                code_TM.kinetic_energy.type = int.Parse(TheValue);
                                break;
                            case "type_interpolation_constrain":
                                code_TM.kinetic_energy.type_interpolation_constrain = int.Parse(TheValue);
                                break;
                            case "type_secondary":
                                code_TM.kinetic_energy.type_secondary = int.Parse(TheValue);
                                break;
                            case "type_of_vertical_profile":
                                code_TM.kinetic_energy.type_of_vertical_profile = int.Parse(TheValue);
                                break;
                            case "format":
                                code_TM.kinetic_energy.format = int.Parse(TheValue);
                                break;
                            case "constant_value":
                                code_TM.kinetic_energy.constant_value = float.Parse(TheValue);
                                break;
                            case "file_name":
                                code_TM.kinetic_energy.file_name = TheValue;
                                break;
                            case "item_number":
                                code_TM.kinetic_energy.item_number = int.Parse(TheValue);
                                break;
                            case "item_name":
                                code_TM.kinetic_energy.item_name = TheValue;
                                break;
                            case "type_of_soft_start":
                                code_TM.kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                                break;
                            case "soft_time_interval":
                                code_TM.kinetic_energy.soft_time_interval = int.Parse(TheValue);
                                break;
                            case "reference_value":
                                code_TM.kinetic_energy.reference_value = int.Parse(TheValue);
                                break;
                            case "type_of_time_interpolation":
                                code_TM.kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_space_interpolation":
                                code_TM.kinetic_energy.type_of_space_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_coriolis_correction":
                                code_TM.kinetic_energy.type_of_coriolis_correction = int.Parse(TheValue);
                                break;
                            case "type_of_wind_correction":
                                code_TM.kinetic_energy.type_of_wind_correction = int.Parse(TheValue);
                                break;
                            case "type_of_tilting":
                                code_TM.kinetic_energy.type_of_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_tilting_point":
                                code_TM.kinetic_energy.type_of_tilting_point = int.Parse(TheValue);
                                break;
                            case "point_tilting":
                                code_TM.kinetic_energy.point_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_radiation_stress_correction":
                                {
                                    if (count == 0)
                                    {
                                        code_TM.kinetic_energy.type_of_radiation_stress_correction = int.Parse(TheValue);
                                        VariableName = "aaaaaa";
                                    }
                                    else
                                    {
                                        code_TM.kinetic_energy.type_of_radiation_stress_correction2 = int.Parse(TheValue);
                                    }
                                    count += 1;
                                }
                                break;
                            case "type_of_pressure_correction":
                                code_TM.kinetic_energy.type_of_pressure_correction = int.Parse(TheValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    if (!CheckNextLine(sr, @"")) return false;

                    VariableName = "";
                    if (!CheckNextLine(sr, "[DISSIPATION_OF_KINETIC_ENERGY]")) return false;
                    code_TM.dissipation_of_kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.BOUNDARY_CONDITIONS.CODE.DISSIPATION_OF_KINETIC_ENERGY();
                    count = 0;
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // DISSIPATION_OF_KINETIC_ENERGY") break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "identifier":
                                code_TM.dissipation_of_kinetic_energy.identifier = TheValue;
                                break;
                            case "type":
                                code_TM.dissipation_of_kinetic_energy.type = int.Parse(TheValue);
                                break;
                            case "type_interpolation_constrain":
                                code_TM.dissipation_of_kinetic_energy.type_interpolation_constrain = int.Parse(TheValue);
                                break;
                            case "type_secondary":
                                code_TM.dissipation_of_kinetic_energy.type_secondary = int.Parse(TheValue);
                                break;
                            case "type_of_vertical_profile":
                                code_TM.dissipation_of_kinetic_energy.type_of_vertical_profile = int.Parse(TheValue);
                                break;
                            case "format":
                                code_TM.dissipation_of_kinetic_energy.format = int.Parse(TheValue);
                                break;
                            case "constant_value":
                                code_TM.dissipation_of_kinetic_energy.constant_value = float.Parse(TheValue);
                                break;
                            case "file_name":
                                code_TM.dissipation_of_kinetic_energy.file_name = TheValue;
                                break;
                            case "item_number":
                                code_TM.dissipation_of_kinetic_energy.item_number = int.Parse(TheValue);
                                break;
                            case "item_name":
                                code_TM.dissipation_of_kinetic_energy.item_name = TheValue;
                                break;
                            case "type_of_soft_start":
                                code_TM.dissipation_of_kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                                break;
                            case "soft_time_interval":
                                code_TM.dissipation_of_kinetic_energy.soft_time_interval = int.Parse(TheValue);
                                break;
                            case "reference_value":
                                code_TM.dissipation_of_kinetic_energy.reference_value = int.Parse(TheValue);
                                break;
                            case "type_of_time_interpolation":
                                code_TM.dissipation_of_kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_space_interpolation":
                                code_TM.dissipation_of_kinetic_energy.type_of_space_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_coriolis_correction":
                                code_TM.dissipation_of_kinetic_energy.type_of_coriolis_correction = int.Parse(TheValue);
                                break;
                            case "type_of_wind_correction":
                                code_TM.dissipation_of_kinetic_energy.type_of_wind_correction = int.Parse(TheValue);
                                break;
                            case "type_of_tilting":
                                code_TM.dissipation_of_kinetic_energy.type_of_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_tilting_point":
                                code_TM.dissipation_of_kinetic_energy.type_of_tilting_point = int.Parse(TheValue);
                                break;
                            case "point_tilting":
                                code_TM.dissipation_of_kinetic_energy.point_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_radiation_stress_correction":
                                {
                                    if (count == 0)
                                    {
                                        code_TM.dissipation_of_kinetic_energy.type_of_radiation_stress_correction = int.Parse(TheValue);
                                        VariableName = "aaaaaa";
                                    }
                                    else
                                    {
                                        code_TM.dissipation_of_kinetic_energy.type_of_radiation_stress_correction2 = int.Parse(TheValue);
                                    }
                                    count += 1;
                                }
                                break;
                            case "type_of_pressure_correction":
                                code_TM.dissipation_of_kinetic_energy.type_of_pressure_correction = int.Parse(TheValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    if (!CheckNextLine(sr, @"")) return false;
                }

                femEngineHD.hydrodynamic_module.turbulence_module.boundary_conditions.code.Add(CodeStr, code_TM);

                if (!CheckNextLine(sr, "EndSect  // " + CodeStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[INITIAL_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.INITIAL_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[KINETIC_ENERGY]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.Touched = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS_KINETIC_ENERGY(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS_DISSIPATION_OF_KINETIC_ENERGY(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, "EndSect  // INITIAL_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS_DISSIPATION_OF_KINETIC_ENERGY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DISSIPATION_OF_KINETIC_ENERGY]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.INITIAL_CONDITIONS.DISSIPATION_OF_KINETIC_ENERGY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DISSIPATION_OF_KINETIC_ENERGY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.dissipation_of_kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_INITIAL_CONDITIONS_KINETIC_ENERGY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[KINETIC_ENERGY]") return false;
            femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.INITIAL_CONDITIONS.KINETIC_ENERGY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // KINETIC_ENERGY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.turbulence_module.initial_conditions.kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOURCES_TM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOURCES]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.sources = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[SOURCE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.sources.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.turbulence_module.sources.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOURCES_TM_SOURCE_TM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // SOURCES")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOURCES_TM_SOURCE_TM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string SourceStr = "";

            femEngineHD.hydrodynamic_module.turbulence_module.sources.source = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES.SOURCE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.turbulence_module.sources.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                SourceStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES.SOURCE source_TM = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES.SOURCE();

                VariableName = "";
                if (!CheckNextLine(sr, "[KINETIC_ENERGY]")) return false;
                source_TM.kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES.SOURCE.KINETIC_ENERGY();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // KINETIC_ENERGY") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            source_TM.kinetic_energy.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            source_TM.kinetic_energy.type = int.Parse(TheValue);
                            break;
                        case "format":
                            source_TM.kinetic_energy.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            source_TM.kinetic_energy.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            source_TM.kinetic_energy.file_name = TheValue;
                            break;
                        case "item_number":
                            source_TM.kinetic_energy.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            source_TM.kinetic_energy.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            source_TM.kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            source_TM.kinetic_energy.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            source_TM.kinetic_energy.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            source_TM.kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!CheckNextLine(sr, @"")) return false;

                VariableName = "";
                if (!CheckNextLine(sr, "[DISSIPATION_OF_KINETIC_ENERGY]")) return false;
                source_TM.dissipation_of_kinetic_energy = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOURCES.SOURCE.DISSIPATION_OF_KINETIC_ENERGY();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // DISSIPATION_OF_KINETIC_ENERGY") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            source_TM.dissipation_of_kinetic_energy.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            source_TM.dissipation_of_kinetic_energy.type = int.Parse(TheValue);
                            break;
                        case "format":
                            source_TM.dissipation_of_kinetic_energy.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            source_TM.dissipation_of_kinetic_energy.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            source_TM.dissipation_of_kinetic_energy.file_name = TheValue;
                            break;
                        case "item_number":
                            source_TM.dissipation_of_kinetic_energy.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            source_TM.dissipation_of_kinetic_energy.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            source_TM.dissipation_of_kinetic_energy.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            source_TM.dissipation_of_kinetic_energy.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            source_TM.dissipation_of_kinetic_energy.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            source_TM.dissipation_of_kinetic_energy.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!CheckNextLine(sr, @"")) return false;


                femEngineHD.hydrodynamic_module.turbulence_module.sources.source.Add(SourceStr, source_TM);

                if (!CheckNextLine(sr, @"EndSect  // " + SourceStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_DIFFUSION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DIFFUSION]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.diffusion = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.DIFFUSION();
            // int count = 0;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DIFFUSION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.diffusion.Touched = int.Parse(TheValue);
                        break;
                    case "sigma_k_h":
                        femEngineHD.hydrodynamic_module.turbulence_module.diffusion.sigma_k_h = float.Parse(TheValue);
                        break;
                    case "sigma_e_h":
                        femEngineHD.hydrodynamic_module.turbulence_module.diffusion.sigma_e_h = float.Parse(TheValue);
                        break;
                    case "sigma_k":
                        femEngineHD.hydrodynamic_module.turbulence_module.diffusion.sigma_k = float.Parse(TheValue);
                        break;
                    case "sigma_e":
                        femEngineHD.hydrodynamic_module.turbulence_module.diffusion.sigma_e = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SOLUTION_TECHNIQUE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOLUTION_TECHNIQUE]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.solution_technique = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SOLUTION_TECHNIQUE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SOLUTION_TECHNIQUE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.solution_technique.Touched = int.Parse(TheValue);
                        break;
                    case "scheme_of_time_integration":
                        femEngineHD.hydrodynamic_module.turbulence_module.solution_technique.scheme_of_time_integration = int.Parse(TheValue);
                        break;
                    case "scheme_of_space_discretization_horizontal":
                        femEngineHD.hydrodynamic_module.turbulence_module.solution_technique.scheme_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    case "scheme_of_space_discretization_vertical":
                        femEngineHD.hydrodynamic_module.turbulence_module.solution_technique.scheme_of_space_discretization_vertical = int.Parse(TheValue);
                        break;
                    case "method_of_space_discretization_horizontal":
                        femEngineHD.hydrodynamic_module.turbulence_module.solution_technique.method_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_EQUATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[EQUATION]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.equation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.EQUATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EQUATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.Touched = int.Parse(TheValue);
                        break;
                    case "c1e":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.c1e = float.Parse(TheValue);
                        break;
                    case "c2e":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.c2e = float.Parse(TheValue);
                        break;
                    case "c3e":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.c3e = float.Parse(TheValue);
                        break;
                    case "prandtl_number":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.prandtl_number = float.Parse(TheValue);
                        break;
                    case "cmy":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.cmy = float.Parse(TheValue);
                        break;
                    case "minimum_kinetic_energy":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.minimum_kinetic_energy = double.Parse(TheValue);
                        break;
                    case "maximum_kinetic_energy":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.maximum_kinetic_energy = double.Parse(TheValue);
                        break;
                    case "minimum_dissipation_of_kinetic_energy":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.minimum_dissipation_of_kinetic_energy = double.Parse(TheValue);
                        break;
                    case "maximum_dissipation_of_kinetic_energy":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.maximum_dissipation_of_kinetic_energy = double.Parse(TheValue);
                        break;
                    case "surface_dissipation_parameter":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.surface_dissipation_parameter = double.Parse(TheValue);
                        break;
                    case "Ri_damping":
                        femEngineHD.hydrodynamic_module.turbulence_module.equation.Ri_damping = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_SPACE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SPACE]")) return false;
            femEngineHD.hydrodynamic_module.turbulence_module.space = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.SPACE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SPACE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_2D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.turbulence_module.space.number_of_2D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.turbulence_module.space.number_of_3D_mesh_geometry = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TURBULENCE_MODULE_TIME(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != @"[TIME]") return false;
            femEngineHD.hydrodynamic_module.turbulence_module.time = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TURBULENCE_MODULE.TIME();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TIME") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "start_time_step":
                        femEngineHD.hydrodynamic_module.turbulence_module.time.start_time_step = int.Parse(TheValue);
                        break;
                    case "time_step_factor":
                        femEngineHD.hydrodynamic_module.turbulence_module.time.time_step_factor = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[TEMPERATURE_SALINITY_MODULE]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[TIME]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "temperature_mode":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.temperature_mode = int.Parse(TheValue);
                        break;
                    case "salinity_mode":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.salinity_mode = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_TIME(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SPACE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_EQUATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOLUTION_TECHNIQUE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOURCES_TSM(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_INITIAL_CONDITIONS(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_BOUNDARY_CONDITIONS(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, "EndSect  // TEMPERATURE_SALINITY_MODULE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_BOUNDARY_CONDITIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[BOUNDARY_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[CODE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_BOUNDARY_CONDITIONS_CODE_BC(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // BOUNDARY_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_BOUNDARY_CONDITIONS_CODE_BC(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string CodeStr = "";

            femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions.code = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions.MzSEPfsListItemCount + 1; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                CodeStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE code = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE();
                if (CodeStr == "CODE_1")
                {
                    if (!CheckNextLine(sr, "[TEMPERATURE]")) return false;
                    code.temperature = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE.TEMPERATURE();
                    if (!CheckNextLine(sr, @"EndSect  // TEMPERATURE")) return false;
                    if (!CheckNextLine(sr, "")) return false;
                    if (!CheckNextLine(sr, "[SALINITY]")) return false;
                    code.salinity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE.SALINITY();
                    if (!CheckNextLine(sr, @"EndSect  // SALINITY")) return false;
                    if (!CheckNextLine(sr, "")) return false;
                }
                else
                {
                    if (!CheckNextLine(sr, "[TEMPERATURE]")) return false;
                    code.temperature = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE.TEMPERATURE();
                    int count = 0;
                    VariableName = "";
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // TEMPERATURE") break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "identifier":
                                code.temperature.identifier = TheValue;
                                break;
                            case "type":
                                code.temperature.type = int.Parse(TheValue);
                                break;
                            case "type_interpolation_constrain":
                                code.temperature.type_interpolation_constrain = int.Parse(TheValue);
                                break;
                            case "type_secondary":
                                code.temperature.type_secondary = int.Parse(TheValue);
                                break;
                            case "type_of_vertical_profile":
                                code.temperature.type_of_vertical_profile = int.Parse(TheValue);
                                break;
                            case "format":
                                code.temperature.format = int.Parse(TheValue);
                                break;
                            case "constant_value":
                                code.temperature.constant_value = float.Parse(TheValue);
                                break;
                            case "file_name":
                                code.temperature.file_name = TheValue;
                                break;
                            case "item_number":
                                code.temperature.item_number = int.Parse(TheValue);
                                break;
                            case "item_name":
                                code.temperature.item_name = TheValue;
                                break;
                            case "type_of_soft_start":
                                code.temperature.type_of_soft_start = int.Parse(TheValue);
                                break;
                            case "soft_time_interval":
                                code.temperature.soft_time_interval = int.Parse(TheValue);
                                break;
                            case "reference_value":
                                code.temperature.reference_value = int.Parse(TheValue);
                                break;
                            case "type_of_time_interpolation":
                                code.temperature.type_of_time_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_space_interpolation":
                                code.temperature.type_of_space_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_coriolis_correction":
                                code.temperature.type_of_coriolis_correction = int.Parse(TheValue);
                                break;
                            case "type_of_wind_correction":
                                code.temperature.type_of_wind_correction = int.Parse(TheValue);
                                break;
                            case "type_of_tilting":
                                code.temperature.type_of_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_tilting_point":
                                code.temperature.type_of_tilting_point = int.Parse(TheValue);
                                break;
                            case "point_tilting":
                                code.temperature.point_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_radiation_stress_correction":
                                {
                                    if (count == 0)
                                    {
                                        code.temperature.type_of_radiation_stress_correction = int.Parse(TheValue);
                                        TheLine = "aaaaaaaa";
                                    }
                                    else
                                    {
                                        code.temperature.type_of_radiation_stress_correction2 = int.Parse(TheValue);
                                    }
                                    count += 1;
                                }
                                break;
                            case "type_of_pressure_correction":
                                code.temperature.type_of_pressure_correction = int.Parse(TheValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    if (!CheckNextLine(sr, "")) return false;
                    if (!CheckNextLine(sr, "[SALINITY]")) return false;
                    code.salinity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.BOUNDARY_CONDITIONS.CODE.SALINITY();
                    count = 0;
                    TheLine = "";
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // SALINITY") break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "identifier":
                                code.salinity.identifier = TheValue;
                                break;
                            case "type":
                                code.salinity.type = int.Parse(TheValue);
                                break;
                            case "type_interpolation_constrain":
                                code.salinity.type_interpolation_constrain = int.Parse(TheValue);
                                break;
                            case "type_secondary":
                                code.salinity.type_secondary = int.Parse(TheValue);
                                break;
                            case "type_of_vertical_profile":
                                code.salinity.type_of_vertical_profile = int.Parse(TheValue);
                                break;
                            case "format":
                                code.salinity.format = int.Parse(TheValue);
                                break;
                            case "constant_value":
                                code.salinity.constant_value = float.Parse(TheValue);
                                break;
                            case "file_name":
                                code.salinity.file_name = TheValue;
                                break;
                            case "item_number":
                                code.salinity.item_number = int.Parse(TheValue);
                                break;
                            case "item_name":
                                code.salinity.item_name = TheValue;
                                break;
                            case "type_of_soft_start":
                                code.salinity.type_of_soft_start = int.Parse(TheValue);
                                break;
                            case "soft_time_interval":
                                code.salinity.soft_time_interval = int.Parse(TheValue);
                                break;
                            case "reference_value":
                                code.salinity.reference_value = int.Parse(TheValue);
                                break;
                            case "type_of_time_interpolation":
                                code.salinity.type_of_time_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_space_interpolation":
                                code.salinity.type_of_space_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_coriolis_correction":
                                code.salinity.type_of_coriolis_correction = int.Parse(TheValue);
                                break;
                            case "type_of_wind_correction":
                                code.salinity.type_of_wind_correction = int.Parse(TheValue);
                                break;
                            case "type_of_tilting":
                                code.salinity.type_of_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_tilting_point":
                                code.salinity.type_of_tilting_point = int.Parse(TheValue);
                                break;
                            case "point_tilting":
                                code.salinity.point_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_radiation_stress_correction":
                                {
                                    if (count == 0)
                                    {
                                        code.salinity.type_of_radiation_stress_correction = int.Parse(TheValue);
                                        TheLine = "aaaaaaa";
                                    }
                                    else
                                    {
                                        code.salinity.type_of_radiation_stress_correction2 = int.Parse(TheValue);
                                    }
                                    count += 1;
                                }
                                break;
                            case "type_of_pressure_correction":
                                code.salinity.type_of_pressure_correction = int.Parse(TheValue);
                                break;
                            default:
                                return false;
                        }
                    }
                    if (!CheckNextLine(sr, "")) return false;
                }

                femEngineHD.hydrodynamic_module.temperature_salinity_module.boundary_conditions.code.Add(CodeStr, code);

                if (!CheckNextLine(sr, @"EndSect  // " + CodeStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_INITIAL_CONDITIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[INITIAL_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.INITIAL_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[TEMPERATURE]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.Touched = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }

            VariableName = "";
            femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.INITIAL_CONDITIONS.TEMPERATURE_TSM();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TEMPERATURE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.temperature.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;

            VariableName = "";
            if (!CheckNextLine(sr, "[SALINITY]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.INITIAL_CONDITIONS.SALINITY_TSM();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SALINITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.salinity.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;

            if (!CheckNextLine(sr, @"EndSect  // INITIAL_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_INITIAL_CONDITIONS_TEMPERATURE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[TEMPERATURE]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.INITIAL_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TEMPERATURE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.initial_conditions.Touched = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_INITIAL_CONDITIONS_TEMPERATURE(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOURCES_TSM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOURCES]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.sources = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[SOURCE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.sources.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.sources.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOURCES_TSM_SOURCE_TSM(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, "EndSect  // SOURCES")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOURCES_TSM_SOURCE_TSM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string SourceStr = "";

            femEngineHD.hydrodynamic_module.temperature_salinity_module.sources.source = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES.SOURCE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.temperature_salinity_module.sources.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                SourceStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES.SOURCE source_TSM = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES.SOURCE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == "[TEMPERATURE]") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "name":
                            source_TSM.name = TheValue;
                            break;
                        case "type_of_temperature":
                            source_TSM.type_of_temperature = int.Parse(TheValue);
                            break;
                        case "type_of_salinity":
                            source_TSM.type_of_salinity = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }

                VariableName = "";
                source_TSM.temperature = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES.SOURCE.TEMPERATURE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // TEMPERATURE") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            source_TSM.temperature.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            source_TSM.temperature.type = int.Parse(TheValue);
                            break;
                        case "format":
                            source_TSM.temperature.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            source_TSM.temperature.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            source_TSM.temperature.file_name = TheValue;
                            break;
                        case "item_number":
                            source_TSM.temperature.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            source_TSM.temperature.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            source_TSM.temperature.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            source_TSM.temperature.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            source_TSM.temperature.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            source_TSM.temperature.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!CheckNextLine(sr, @"")) return false;

                VariableName = "";
                if (!CheckNextLine(sr, "[SALINITY]")) return false;
                source_TSM.salinity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOURCES.SOURCE.SALINITY();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // SALINITY") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            source_TSM.salinity.Touched = int.Parse(TheValue);
                            break;
                        case "type":
                            source_TSM.salinity.type = int.Parse(TheValue);
                            break;
                        case "format":
                            source_TSM.salinity.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            source_TSM.salinity.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            source_TSM.salinity.file_name = TheValue;
                            break;
                        case "item_number":
                            source_TSM.salinity.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            source_TSM.salinity.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            source_TSM.salinity.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            source_TSM.salinity.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            source_TSM.salinity.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            source_TSM.salinity.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!CheckNextLine(sr, @"")) return false;


                femEngineHD.hydrodynamic_module.temperature_salinity_module.sources.source.Add(SourceStr, source_TSM);

                if (!CheckNextLine(sr, @"EndSect  // " + SourceStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[PRECIPITATION_EVAPORATION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[PRECIPITATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "type_of_precipitation":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.AmbientWaterTemperature:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.type_of_precipitation = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.AmbientWaterTemperature;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.SpecifiedTemperature:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.type_of_precipitation = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.AmbientWaterTemperature;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "type_of_evaporation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.type_of_evaporation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION_PRECIPITATION(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION_EVAPORATION(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // PRECIPITATION_EVAPORATION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION_EVAPORATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[EVAPORATION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EVAPORATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.evaporation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_PRECIPITATION_EVAPORATION_PRECIPITATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[PRECIPITATION]") return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.PRECIPITATION_EVAPORATION.PRECIPITATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PRECIPITATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.precipitation_evaporation.precipitation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[HEAT_EXCHANGE]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[air_temperature]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.TYPE.HeatExchangeNotIncluded:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.TYPE.HeatExchangeNotIncluded;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.TYPE.HeatExchangeIncluded:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.TYPE.HeatExchangeIncluded;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "Angstroms_law_A":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Angstroms_law_A = float.Parse(TheValue);
                        break;
                    case "Angstroms_law_B":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Angstroms_law_B = float.Parse(TheValue);
                        break;
                    case "Beers_law_beta":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Beers_law_beta = float.Parse(TheValue);
                        break;
                    case "light_extinction":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.light_extinction = float.Parse(TheValue);
                        break;
                    case "displacement_hours":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.displacement_hours = float.Parse(TheValue);
                        break;
                    case "standard_meridian":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.standard_meridian = float.Parse(TheValue);
                        break;
                    case "Daltons_law_A":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Daltons_law_A = float.Parse(TheValue);
                        break;
                    case "Daltons_law_B":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.Daltons_law_B = float.Parse(TheValue);
                        break;
                    case "sensible_heat_transfer_coefficient_heating":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.sensible_heat_transfer_coefficient_heating = float.Parse(TheValue);
                        break;
                    case "sensible_heat_transfer_coefficient_cooling":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.sensible_heat_transfer_coefficient_cooling = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_AIR_TEMPERATURE(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_RELATIVE_HUMIDITY(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_CLEARNESS_COEFFICIENT(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // HEAT_EXCHANGE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_CLEARNESS_COEFFICIENT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[clearness_coefficient]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.CLEARNESS_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // clearness_coefficient") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.clearness_coefficient.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_RELATIVE_HUMIDITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[relative_humidity]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.RELATIVE_HUMIDITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // relative_humidity") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.relative_humidity.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_HEAT_EXCHANGE_AIR_TEMPERATURE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[air_temperature]") return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.HEAT_EXCHANGE.AIR_TEMPERATURE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // air_temperature") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.heat_exchange.air_temperature.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            if (!CheckNextLine(sr, "[DIFFUSION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION();
            Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION(sr, femEngineHD);
            Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION(sr, femEngineHD);
            if (!CheckNextLine(sr, @"EndSect  // DIFFUSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[VERTICAL_DIFFUSION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.VERTICAL_DIFFUSION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[SCALED_EDDY_VISCOSITY]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.type = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION_SCALED_EDDY_VISCOSITY(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION_DIFFUSION_COEFFICIENT(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // VERTICAL_DIFFUSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION_DIFFUSION_COEFFICIENT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DIFFUSION_COEFFICIENT]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.VERTICAL_DIFFUSION.DIFFUSION_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DIFFUSION_COEFFICIENT") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.diffusion_coefficient.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_VERTICAL_DIFFUSION_SCALED_EDDY_VISCOSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[SCALED_EDDY_VISCOSITY]") return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.VERTICAL_DIFFUSION.SCALED_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SCALED_EDDY_VISCOSITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity.format = int.Parse(TheValue);
                        break;
                    case "sigma":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity.sigma = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.vertical_diffusion.scaled_eddy_viscosity.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[HORIZONTAL_DIFFUSION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[SCALED_EDDY_VISCOSITY]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.NoDispersion:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.NoDispersion;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.ScaledEddyViscosityFormulation:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.ScaledEddyViscosityFormulation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.DispersionViscosityFormulation:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.TYPE.DispersionViscosityFormulation;
                                break;
                            default:
                                return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION_SCALED_EDDY_VISCOSITY(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION_DIFFUSION_COEFFICIENT(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // HORIZONTAL_DIFFUSION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION_DIFFUSION_COEFFICIENT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DIFFUSION_COEFFICIENT]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.DIFFUSION_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DIFFUSION_COEFFICIENT") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.diffusion_coefficient.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_DIFFUSION_HORIZONTAL_DIFFUSION_SCALED_EDDY_VISCOSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[SCALED_EDDY_VISCOSITY]") return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.DIFFUSION.HORIZONTAL_DIFFUSION.SCALED_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SCALED_EDDY_VISCOSITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity.format = int.Parse(TheValue);
                        break;
                    case "sigma":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity.sigma = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.diffusion.horizontal_diffusion.scaled_eddy_viscosity.item_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SOLUTION_TECHNIQUE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOLUTION_TECHNIQUE]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SOLUTION_TECHNIQUE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.Touched = int.Parse(TheValue);
                        break;
                    case "scheme_of_time_integration":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.LowOrderFastAlgorithm:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.scheme_of_time_integration = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.LowOrderFastAlgorithm;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.HigherOrder:
                                femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.scheme_of_time_integration = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.HigherOrder;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "scheme_of_space_discretization_horizontal":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.scheme_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    case "scheme_of_space_discretization_vertical":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.scheme_of_space_discretization_vertical = int.Parse(TheValue);
                        break;
                    case "method_of_space_discretization_horizontal":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.solution_technique.method_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_EQUATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[EQUATION]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.EQUATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EQUATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM.Touched = int.Parse(TheValue);
                        break;
                    case "minimum_temperature":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM.minimum_temperature = float.Parse(TheValue);
                        break;
                    case "maximum_temperature":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM.maximum_temperature = float.Parse(TheValue);
                        break;
                    case "minimum_salinity":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM.minimum_salinity = float.Parse(TheValue);
                        break;
                    case "maximum_salinity":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.equation_TSM.maximum_salinity = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_SPACE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SPACE]")) return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.space = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.SPACE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SPACE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_2D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.space.number_of_2D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.space.number_of_3D_mesh_geometry = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TEMPERATURE_SALINITY_MODULE_TIME(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != @"[TIME]") return false;
            femEngineHD.hydrodynamic_module.temperature_salinity_module.time = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TEMPERATURE_SALINITY_MODULE.TIME();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TIME") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "start_time_step":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.time.start_time_step = int.Parse(TheValue);
                        break;
                    case "time_step_factor":
                        femEngineHD.hydrodynamic_module.temperature_salinity_module.time.time_step_factor = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BOUNDARY_CONDITIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[BOUNDARY_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.boundary_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[CODE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.boundary_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.boundary_conditions.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "internal_land_boundary_Type":
                        femEngineHD.hydrodynamic_module.boundary_conditions.internal_land_boundary_Type = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_BOUNDARY_CONDITIONS_CODE(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // BOUNDARY_CONDITIONS")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_BOUNDARY_CONDITIONS_CODE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string CodeStr = "";

            femEngineHD.hydrodynamic_module.boundary_conditions.code = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.boundary_conditions.MzSEPfsListItemCount + 1; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                CodeStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE code = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE();
                if (CodeStr == "CODE_1")
                {
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // " + CodeStr) break;
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                        switch (VariableName)
                        {
                            case "type":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroNormalVelocity:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroNormalVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroVelocity:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedVelocity:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedFluxes:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedFluxes;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedLevel:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedLevel;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedDischarge:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedDischarge;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.FlatherCondition:
                                        code.type =
                                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.FlatherCondition;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            default:
                                return false;
                        }
                    }
                }
                else
                {
                    int count = 0;
                    while (true)
                    {
                        TheLine = GetTheLine(sr);
                        if (TheLine == @"EndSect  // " + CodeStr) break;
                        if (TheLine.StartsWith(@"[DATA_"))
                        {
                            code.data = new List<m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.DATA>();
                            while (true)
                            {
                                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_BOUNDARY_CONDITIONS_CODE_DATA(sr, femEngineHD, code, TheLine)) return false;
                                TheLine = GetTheLine(sr);
                                if (!TheLine.StartsWith("[DATA_")) break;
                            }
                        }
                        VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                        TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();

                        switch (VariableName)
                        {
                            case "identifier":
                                code.identifier = TheValue;
                                break;
                            case "type":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroNormalVelocity:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroNormalVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroVelocity:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.LandZeroVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedVelocity:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedVelocity;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedFluxes:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedFluxes;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedLevel:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedLevel;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedDischarge:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.SpecifiedDischarge;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.FlatherCondition:
                                        code.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE.FlatherCondition;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "type_interpolation_constrain":
                                code.type_interpolation_constrain = int.Parse(TheValue);
                                break;
                            case "type_secondary":
                                code.type_secondary = int.Parse(TheValue);
                                break;
                            case "type_of_vertical_profile":
                                code.type_of_vertical_profile = int.Parse(TheValue);
                                break;
                            case "format":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.Constant:
                                        code.format = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.Constant;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.VaryingInTimeConstantAlongBoundary:
                                        code.format = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.VaryingInTimeConstantAlongBoundary;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.VaryingInTimeAndAlongBoundary:
                                        code.format = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.FORMAT.VaryingInTimeAndAlongBoundary;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "constant_value":
                                code.constant_value = float.Parse(TheValue);
                                break;
                            case "constant_values":
                                {
                                    code.constant_values = new List<float>();
                                    foreach (string s in TheValue.Split(delimiter))
                                    {
                                        code.constant_values.Add(float.Parse(s));
                                    }
                                }
                                break;
                            case "file_name":
                                code.file_name = TheValue;
                                break;
                            case "item_number":
                                code.item_number = int.Parse(TheValue);
                                break;
                            case "item_numbers":
                                {
                                    code.item_numbers = new List<int>();
                                    foreach (string s in TheValue.Split(delimiter))
                                    {
                                        code.item_numbers.Add(int.Parse(s));
                                    }
                                }
                                break;
                            case "item_name":
                                code.item_name = TheValue;
                                break;
                            case "item_names":
                                {
                                    code.item_names = new List<string>();
                                    foreach (string s in TheValue.Split(delimiter))
                                    {
                                        // potential errors what happens if within the name there is a ","
                                        code.item_names.Add(s);
                                    }
                                }
                                break;
                            case "type_of_soft_start":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_SOFT_START)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_SOFT_START.LinearVariation:
                                        code.type_of_soft_start = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_SOFT_START.LinearVariation;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_SOFT_START.SinusVariation:
                                        code.type_of_soft_start = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_SOFT_START.SinusVariation;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "soft_time_interval":
                                code.soft_time_interval = int.Parse(TheValue);
                                break;
                            case "reference_value":
                                code.reference_value = float.Parse(TheValue);
                                break;
                            case "reference_values":
                                {
                                    code.reference_values = new List<float>();
                                    foreach (string s in TheValue.Split(delimiter))
                                    {
                                        code.reference_values.Add(int.Parse(s));
                                    }
                                }
                                break;
                            case "type_of_time_interpolation":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_TIME_INTERPOLATION)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_TIME_INTERPOLATION.Linear:
                                        code.type_of_time_interpolation = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_TIME_INTERPOLATION.Linear;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_TIME_INTERPOLATION.PiecewiseCubic:
                                        code.type_of_time_interpolation = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_TIME_INTERPOLATION.PiecewiseCubic;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "type_of_space_interpolation":
                                code.type_of_space_interpolation = int.Parse(TheValue);
                                break;
                            case "type_of_coriolis_correction":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_CORIOLIS_CORRECTION)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_CORIOLIS_CORRECTION.CoriolisCorrectionNotIncluded:
                                        code.type_of_coriolis_correction = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_CORIOLIS_CORRECTION.CoriolisCorrectionNotIncluded;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_CORIOLIS_CORRECTION.CoriolisCorrectionIncluded:
                                        code.type_of_coriolis_correction = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_CORIOLIS_CORRECTION.CoriolisCorrectionIncluded;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "type_of_wind_correction":
                                switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_WIND_CORRECTION)int.Parse(TheValue))
                                {
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_WIND_CORRECTION.WindCorrectionNotIncluded:
                                        code.type_of_wind_correction = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_WIND_CORRECTION.WindCorrectionNotIncluded;
                                        break;
                                    case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_WIND_CORRECTION.WindCorrectionIncluded:
                                        code.type_of_wind_correction = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.TYPE_OF_WIND_CORRECTION.WindCorrectionIncluded;
                                        break;
                                    default:
                                        return false;
                                }
                                break;
                            case "type_of_tilting":
                                code.type_of_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_tilting_point":
                                code.type_of_tilting_point = int.Parse(TheValue);
                                break;
                            case "point_tilting":
                                code.point_tilting = int.Parse(TheValue);
                                break;
                            case "type_of_radiation_stress_correction":
                                {
                                    if (count == 0)
                                    {
                                        code.type_of_radiation_stress_correction = int.Parse(TheValue);
                                        TheLine = "aaaaa";
                                    }
                                    else
                                    {
                                        code.type_of_radiation_stress_correction2 = int.Parse(TheValue);
                                    }
                                    count += 1;
                                }
                                break;
                            case "type_of_pressure_correction":
                                {
                                    code.type_of_pressure_correction = int.Parse(TheValue);
                                    count += 1;
                                }
                                break;
                            default:
                                return false;
                        }
                    }
                }
                if (!CheckNextLine(sr, @"")) return false;
                femEngineHD.hydrodynamic_module.boundary_conditions.code.Add(CodeStr, code);

            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_BOUNDARY_CONDITIONS_CODE_DATA(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE code, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string DataStr = "";

            if (!TheLine.StartsWith("[DATA_")) return false;
            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.DATA boundary_conditions_hd_code_hd_data = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BOUNDARY_CONDITIONS.CODE.DATA();
            DataStr = TheLine.Substring(1, TheLine.Length - 2);

            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // " + DataStr) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "format":
                        boundary_conditions_hd_code_hd_data.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        boundary_conditions_hd_code_hd_data.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        boundary_conditions_hd_code_hd_data.file_name = TheValue;
                        break;
                    case "item_number":
                        boundary_conditions_hd_code_hd_data.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        boundary_conditions_hd_code_hd_data.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        boundary_conditions_hd_code_hd_data.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        boundary_conditions_hd_code_hd_data.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        boundary_conditions_hd_code_hd_data.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        boundary_conditions_hd_code_hd_data.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    case "type_of_space_interpolation":
                        boundary_conditions_hd_code_hd_data.type_of_space_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            code.data.Add(boundary_conditions_hd_code_hd_data);
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_INITIAL_CONDITIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[INITIAL_CONDITIONS]")) return false;
            femEngineHD.hydrodynamic_module.initial_conditions = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.INITIAL_CONDITIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // INITIAL_CONDITIONS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.initial_conditions.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.initial_conditions.type = int.Parse(TheValue);
                        break;
                    case "surface_elevation_constant":
                        femEngineHD.hydrodynamic_module.initial_conditions.surface_elevation_constant = float.Parse(TheValue);
                        break;
                    case "u_velocity_constant":
                        femEngineHD.hydrodynamic_module.initial_conditions.u_velocity_constant = float.Parse(TheValue);
                        break;
                    case "v_velocity_constant":
                        femEngineHD.hydrodynamic_module.initial_conditions.v_velocity_constant = float.Parse(TheValue);
                        break;
                    case "w_velocity_constant":
                        femEngineHD.hydrodynamic_module.initial_conditions.w_velocity_constant = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[STRUCTURES]")) return false;
            femEngineHD.hydrodynamic_module.structures = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[GATES]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_PIERS_HD(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // STRUCTURES")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            TheLine = GetTheLine(sr);
            if (TheLine != "[TURBINES]") return false;
            femEngineHD.hydrodynamic_module.structures.turbines = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[TURBINE_") || TheLine == @"EndSect  // TURBINES") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.structures.turbines.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.structures.turbines.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.structures.turbines.format = int.Parse(TheValue);
                        break;
                    case "number_of_turbines":
                        femEngineHD.hydrodynamic_module.structures.turbines.number_of_turbines = int.Parse(TheValue);
                        break;
                    case "output_type":
                        femEngineHD.hydrodynamic_module.structures.turbines.output_type = int.Parse(TheValue);
                        break;
                    case "output_frequency":
                        femEngineHD.hydrodynamic_module.structures.turbines.output_frequency = int.Parse(TheValue);
                        break;
                    case "output_file_name":
                        femEngineHD.hydrodynamic_module.structures.turbines.output_file_name = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (TheLine.StartsWith(@"[TURBINE_"))
            {
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD_TURBINE_HD(sr, femEngineHD, TheLine)) return false;
                if (!CheckNextLine(sr, @"EndSect  // TURBINES")) return false;
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD_TURBINE_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string TurbineStr = "";

            femEngineHD.hydrodynamic_module.structures.turbines.turbine = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES.TURBINE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.structures.turbines.number_of_turbines; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                TurbineStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES.TURBINE turbines_hd_turbine_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES.TURBINE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"[CORRECTION_FACTOR]") break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Name":
                            turbines_hd_turbine_hd.Name = TheValue;
                            break;
                        case "include":
                            turbines_hd_turbine_hd.include = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            turbines_hd_turbine_hd.coordinate_type = TheValue;
                            break;
                        case "x":
                            turbines_hd_turbine_hd.x = double.Parse(TheValue);
                            break;
                        case "y":
                            turbines_hd_turbine_hd.y = double.Parse(TheValue);
                            break;
                        case "diameter":
                            turbines_hd_turbine_hd.diameter = float.Parse(TheValue);
                            break;
                        case "centroid":
                            turbines_hd_turbine_hd.centroid = float.Parse(TheValue);
                            break;
                        case "drag_coefficient":
                            turbines_hd_turbine_hd.drag_coefficient = float.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD_TURBINE_HD_CORRECTION_FACTOR_HD(sr, femEngineHD, turbines_hd_turbine_hd, TheLine)) return false;
                femEngineHD.hydrodynamic_module.structures.turbines.turbine.Add(TurbineStr, turbines_hd_turbine_hd);

                if (!CheckNextLine(sr, @"EndSect  // " + TurbineStr)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_TURBINES_HD_TURBINE_HD_CORRECTION_FACTOR_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES.TURBINE turbines_hd_turbine_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (TheLine != "[CORRECTION_FACTOR]") return false;
            turbines_hd_turbine_hd.correction_factor = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.TURBINES.TURBINE.CORRECTION_FACTOR();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CORRECTION_FACTOR") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        turbines_hd_turbine_hd.correction_factor.Touched = int.Parse(TheValue);
                        break;
                    case "format":
                        turbines_hd_turbine_hd.correction_factor.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        turbines_hd_turbine_hd.correction_factor.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        turbines_hd_turbine_hd.correction_factor.file_name = TheValue;
                        break;
                    case "item_number":
                        turbines_hd_turbine_hd.correction_factor.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        turbines_hd_turbine_hd.correction_factor.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        turbines_hd_turbine_hd.correction_factor.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        turbines_hd_turbine_hd.correction_factor.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        turbines_hd_turbine_hd.correction_factor.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        turbines_hd_turbine_hd.correction_factor.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_PIERS_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            TheLine = GetTheLine(sr);
            if (TheLine != "[PIERS]") return false;
            femEngineHD.hydrodynamic_module.structures.piers = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[PIER_") || TheLine == @"EndSect  // PIERS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.structures.piers.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.structures.piers.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.structures.piers.format = int.Parse(TheValue);
                        break;
                    case "number_of_piers":
                        femEngineHD.hydrodynamic_module.structures.piers.number_of_piers = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (TheLine.StartsWith(@"[PIER_"))
            {
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_PIERS_HD_PIER_HD(sr, femEngineHD, TheLine)) return false;
                if (!CheckNextLine(sr, @"EndSect  // PIERS")) return false;
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_PIERS_HD_PIER_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string PierStr = "";

            femEngineHD.hydrodynamic_module.structures.piers.pier = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS.PIER>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.structures.piers.number_of_piers; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                PierStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS.PIER piers_hd_pier_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS.PIER();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + PierStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Name":
                            piers_hd_pier_hd.Name = TheValue;
                            break;
                        case "include":
                            piers_hd_pier_hd.include = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            piers_hd_pier_hd.coordinate_type = TheValue;
                            break;
                        case "x":
                            piers_hd_pier_hd.x = double.Parse(TheValue);
                            break;
                        case "y":
                            piers_hd_pier_hd.y = double.Parse(TheValue);
                            break;
                        case "theta":
                            piers_hd_pier_hd.theta = float.Parse(TheValue);
                            break;
                        case "lamda":
                            piers_hd_pier_hd.lamda = float.Parse(TheValue);
                            break;
                        case "number_of_sections":
                            piers_hd_pier_hd.number_of_sections = int.Parse(TheValue);
                            break;
                        case "type":
                            {
                                piers_hd_pier_hd.type = new List<m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS.PIER.TYPE>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    piers_hd_pier_hd.type.Add((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.PIERS.PIER.TYPE)int.Parse(s));
                                }
                            }
                            break;
                        case "height":
                            {
                                piers_hd_pier_hd.height = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    piers_hd_pier_hd.height.Add(float.Parse(s));
                                }
                            }
                            break;
                        case "length":
                            {
                                piers_hd_pier_hd.length = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    piers_hd_pier_hd.length.Add(float.Parse(s));
                                }
                            }
                            break;
                        case "width":
                            {
                                piers_hd_pier_hd.width = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    piers_hd_pier_hd.width.Add(float.Parse(s));
                                }
                            }
                            break;
                        case "radious":
                            {
                                piers_hd_pier_hd.radious = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    piers_hd_pier_hd.radious.Add(float.Parse(s));
                                }
                            }
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.hydrodynamic_module.structures.piers.pier.Add(PierStr, piers_hd_pier_hd);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[GATES]") return false;
            femEngineHD.hydrodynamic_module.structures.gates = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[GATE_") || TheLine == @"EndSect  // GATES") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.structures.gates.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.structures.gates.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "number_of_gates":
                        femEngineHD.hydrodynamic_module.structures.gates.number_of_gates = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (TheLine.StartsWith(@"[GATE_"))
            {
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD_GATE_HD(sr, femEngineHD, TheLine)) return false;
                if (!CheckNextLine(sr, @"EndSect  // GATES")) return false;
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD_GATE_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string GateStr = "";

            femEngineHD.hydrodynamic_module.structures.gates.gate = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.structures.gates.number_of_gates; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                GateStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE gates_hd_gate_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine.StartsWith("[POINT_")) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Name":
                            gates_hd_gate_hd.Name = TheValue;
                            break;
                        case "include":
                            gates_hd_gate_hd.include = int.Parse(TheValue);
                            break;
                        case "input_format":
                            gates_hd_gate_hd.input_format = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            gates_hd_gate_hd.coordinate_type = TheValue;
                            break;
                        case "number_of_points":
                            gates_hd_gate_hd.number_of_points = int.Parse(TheValue);
                            break;
                        case "x":
                            {
                                gates_hd_gate_hd.x = new List<double>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    gates_hd_gate_hd.x.Add(double.Parse(s));
                                }
                            }
                            break;
                        case "y":
                            {
                                gates_hd_gate_hd.y = new List<double>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    gates_hd_gate_hd.y.Add(double.Parse(s));
                                }
                            }
                            break;
                        default:
                            return false;
                    }
                }
                if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD_GATE_HD_POINT_HD(sr, femEngineHD, gates_hd_gate_hd, TheLine)) return false;
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + GateStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "input_file_name":
                            gates_hd_gate_hd.input_file_name = TheValue;
                            break;
                        case "format":
                            gates_hd_gate_hd.format = int.Parse(TheValue);
                            break;
                        case "constant_value":
                            gates_hd_gate_hd.constant_value = float.Parse(TheValue);
                            break;
                        case "file_name":
                            gates_hd_gate_hd.file_name = TheValue;
                            break;
                        case "item_number":
                            gates_hd_gate_hd.item_number = int.Parse(TheValue);
                            break;
                        case "item_name":
                            gates_hd_gate_hd.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            gates_hd_gate_hd.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            gates_hd_gate_hd.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            gates_hd_gate_hd.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            gates_hd_gate_hd.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.hydrodynamic_module.structures.gates.gate.Add(GateStr, gates_hd_gate_hd);
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURES_HD_GATES_HD_GATE_HD_POINT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE gates_hd_gate_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string PointStr = "";

            gates_hd_gate_hd.point = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE.POINT>();

            for (int i = 0; i < gates_hd_gate_hd.number_of_points; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                PointStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE.POINT gates_hd_gate_hd_point_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURES.GATES.GATE.POINT();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + PointStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "x":
                            gates_hd_gate_hd_point_hd.x = double.Parse(TheValue);
                            break;
                        case "y":
                            gates_hd_gate_hd_point_hd.y = double.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                gates_hd_gate_hd.point.Add(PointStr, gates_hd_gate_hd_point_hd);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[STRUCTURE_MODULE]")) return false;
            femEngineHD.hydrodynamic_module.structure_module = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[CROSSSECTIONS]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Structure_Version":
                        {
                            femEngineHD.hydrodynamic_module.structure_module.Structure_Version = new List<int>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                femEngineHD.hydrodynamic_module.structure_module.Structure_Version.Add(int.Parse(s));
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CROSSSECTIONS(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // STRUCTURE_MODULE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            //string VariableName = "";
            //string TheValue = "";
            if (!CheckNextLine(sr, "[CULVERTS]")) return false;
            femEngineHD.hydrodynamic_module.structure_module.culverts = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS();
            femEngineHD.hydrodynamic_module.structure_module.culverts.culvert_data = new List<m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA>();
            TheLine = GetTheLine(sr);
            if (TheLine != @"EndSect  // CULVERTS")
            {
                if (TheLine != @"[culvert_data]") return false;
                while (true)
                {
                    if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD(sr, femEngineHD, TheLine)) return false;
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // CULVERTS") break;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[culvert_data]") return false;
            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA culverts_hd_culvert_data_hd =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[Geometry]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Location":
                        {
                            culverts_hd_culvert_data_hd.Location = new List<string>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd.Location.Add(s);
                            }
                        }
                        break;
                    case "delhs":
                        culverts_hd_culvert_data_hd.delhs = float.Parse(TheValue);
                        break;
                    case "coordinate_type":
                        culverts_hd_culvert_data_hd.coordinate_type = TheValue;
                        break;
                    case "number_of_points":
                        culverts_hd_culvert_data_hd.number_of_points = int.Parse(TheValue);
                        break;
                    case "x":
                        {
                            culverts_hd_culvert_data_hd.x = new List<double>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd.x.Add(double.Parse(s));
                            }
                        }
                        break;
                    case "y":
                        {
                            culverts_hd_culvert_data_hd.y = new List<double>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd.y.Add(double.Parse(s));
                            }
                        }
                        break;
                    case "HorizOffset":
                        culverts_hd_culvert_data_hd.HorizOffset = float.Parse(TheValue);
                        break;
                    case "Attributes":
                        {
                            List<float> TempFloats = new List<float>();
                            culverts_hd_culvert_data_hd.attributes =
                                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.ATTRIBUTES();

                            foreach (string s in TheValue.Split(delimiter))
                            {
                                TempFloats.Add(float.Parse(s));
                            }
                            culverts_hd_culvert_data_hd.attributes.Upstream = TempFloats[0];
                            culverts_hd_culvert_data_hd.attributes.Downstream = TempFloats[1];
                            culverts_hd_culvert_data_hd.attributes.Length = TempFloats[2];
                            culverts_hd_culvert_data_hd.attributes.Manning_n = TempFloats[3];
                            culverts_hd_culvert_data_hd.attributes.NumberOfCulverts = (int)TempFloats[4];
                            culverts_hd_culvert_data_hd.attributes.valve_regulation = (m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.ATTRIBUTES.VALVE_REGULATION)TempFloats[5];
                            culverts_hd_culvert_data_hd.attributes.section_type = (m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.ATTRIBUTES.SECTION_TYPE)TempFloats[6];
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD_GEOMETRY_HD(sr, femEngineHD, culverts_hd_culvert_data_hd, TheLine)) return false;
            femEngineHD.hydrodynamic_module.structure_module.culverts.culvert_data.Add(culverts_hd_culvert_data_hd);
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // culvert_data") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "HeadLossFactors":
                        {
                            culverts_hd_culvert_data_hd.HeadLossFactors = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd.HeadLossFactors.Add(float.Parse(s));
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD_GEOMETRY_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA culverts_hd_culvert_data_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[Geometry]") return false;
            culverts_hd_culvert_data_hd.geometry = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[Irregular]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.Rectangular:
                                culverts_hd_culvert_data_hd.geometry.Type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.Rectangular;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.Circular:
                                culverts_hd_culvert_data_hd.geometry.Type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.Circular;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.IrregularLevelWidthTable:
                                culverts_hd_culvert_data_hd.geometry.Type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.TYPE.IrregularLevelWidthTable;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "Rectangular":
                        {
                            culverts_hd_culvert_data_hd.geometry.Rectangular = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd.geometry.Rectangular.Add(float.Parse(s));
                            }
                        }
                        break;
                    case "Cicular_Diameter":
                        culverts_hd_culvert_data_hd.geometry.Cicular_Diameter = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD_GEOMETRY_HD_IRREGULAR_HD(sr, femEngineHD, culverts_hd_culvert_data_hd.geometry, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // Geometry")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CULVERT_HD_CULVERT_DATA_HD_GEOMETRY_HD_IRREGULAR_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY culverts_hd_culvert_data_hd_geometry_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[Irregular]") return false;
            culverts_hd_culvert_data_hd_geometry_hd.culverts_hd_culvert_data_hd_geometry_hd_irregular_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.IRREGULAR();
            culverts_hd_culvert_data_hd_geometry_hd.culverts_hd_culvert_data_hd_geometry_hd_irregular_hd.data = new List<m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.IRREGULAR.DATA>();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // Irregular") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Data":
                        {
                            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.IRREGULAR.DATA culverts_hd_culvert_data_hd_geometry_hd_irregular_hd_data = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CULVERTS.CULVERT_DATA.GEOMETRY.IRREGULAR.DATA();
                            culverts_hd_culvert_data_hd_geometry_hd_irregular_hd_data.data = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                culverts_hd_culvert_data_hd_geometry_hd_irregular_hd_data.data.Add(float.Parse(s));
                            }
                            culverts_hd_culvert_data_hd_geometry_hd.culverts_hd_culvert_data_hd_geometry_hd_irregular_hd.data.Add(culverts_hd_culvert_data_hd_geometry_hd_irregular_hd_data);
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            //string VariableName = "";
            //string TheValue = "";
            if (!CheckNextLine(sr, "[WEIR]")) return false;
            femEngineHD.hydrodynamic_module.structure_module.weir = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR();
            femEngineHD.hydrodynamic_module.structure_module.weir.weir_data = new List<m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA>();
            TheLine = GetTheLine(sr);
            if (TheLine != @"EndSect  // WEIR")
            {
                if (TheLine != @"[weir_data]") return false;
                while (true)
                {
                    if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD(sr, femEngineHD, TheLine)) return false;
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // WEIR") break;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[weir_data]") return false;
            m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA weir_weir_data_hd = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[Geometry]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Location":
                        {
                            weir_weir_data_hd.Location = new List<string>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.Location.Add(s);
                            }
                        }
                        break;
                    case "delhs":
                        weir_weir_data_hd.delhs = float.Parse(TheValue);
                        break;
                    case "coordinate_type":
                        weir_weir_data_hd.coordinate_type = TheValue;
                        break;
                    case "number_of_points":
                        weir_weir_data_hd.number_of_points = int.Parse(TheValue);
                        break;
                    case "x":
                        {
                            weir_weir_data_hd.x = new List<double>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.x.Add(double.Parse(s));
                            }
                        }
                        break;
                    case "y":
                        {
                            weir_weir_data_hd.y = new List<double>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.y.Add(double.Parse(s));
                            }
                        }
                        break;
                    case "HorizOffset":
                        weir_weir_data_hd.HorizOffset = float.Parse(TheValue);
                        break;
                    case "Attributes":
                        {
                            List<int> TempInt = new List<int>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                TempInt.Add(int.Parse(s));
                            }
                            weir_weir_data_hd.attributes = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES();
                            switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE)TempInt[0])
                            {
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.BroadCrestedWeir:
                                    weir_weir_data_hd.attributes.type =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.BroadCrestedWeir;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.WeirFormula1:
                                    weir_weir_data_hd.attributes.type =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.WeirFormula1;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.WeirFormula2Honma:
                                    weir_weir_data_hd.attributes.type =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.TYPE.WeirFormula2Honma;
                                    break;
                                default:
                                    return false;
                            }
                            switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE)TempInt[1])
                            {
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.None:
                                    weir_weir_data_hd.attributes.valve =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.None;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.OnlyNegativeFlow:
                                    weir_weir_data_hd.attributes.valve =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.OnlyNegativeFlow;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.OnlyPositiveFlow:
                                    weir_weir_data_hd.attributes.valve =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.OnlyPositiveFlow;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.NoFlow:
                                    weir_weir_data_hd.attributes.valve =
                                        m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.ATTRIBUTES.VALVE.NoFlow;
                                    break;
                                default:
                                    return false;
                            }
                        }
                        break;
                    case "HeadLossFactors":
                        {
                            weir_weir_data_hd.HeadLossFactors = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.HeadLossFactors.Add(float.Parse(s));
                            }
                        }
                        break;
                    case "WeirFormulaParam":
                        {
                            weir_weir_data_hd.WeirFormulaParam = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.WeirFormulaParam.Add(float.Parse(s));
                            }
                        }
                        break;
                    case "WeirFormula2Param":
                        {
                            weir_weir_data_hd.WeirFormula2Param = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.WeirFormula2Param.Add(float.Parse(s));
                            }
                        }
                        break;
                    case "WeirFormula3Param":
                        {
                            weir_weir_data_hd.WeirFormula3Param = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.WeirFormula3Param.Add(float.Parse(s));
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD_GEOMETRY_HD(sr, femEngineHD, weir_weir_data_hd, TheLine)) return false;
            femEngineHD.hydrodynamic_module.structure_module.weir.weir_data.Add(weir_weir_data_hd);
            if (!CheckNextLine(sr, @"EndSect  // weir_data")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD_GEOMETRY_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA weir_weir_data_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[Geometry]") return false;
            weir_weir_data_hd.Geometry = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.GEOMETRY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == "[Level_Width]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Attributes":
                        {
                            weir_weir_data_hd.Geometry.Attributes = new List<int>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                weir_weir_data_hd.Geometry.Attributes.Add(int.Parse(s));
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD_GEOMETRY_HD_LEVEL_WIDTH_HD(sr, femEngineHD, weir_weir_data_hd.Geometry, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // Geometry")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_WEIR_HD_WEIR_DATA_HD_GEOMETRY_HD_LEVEL_WIDTH_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.GEOMETRY weir_weir_data_geometry_hd, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[Level_Width]") return false;
            weir_weir_data_geometry_hd.Level_Width = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.WEIR.WEIR_DATA.GEOMETRY.LEVEL_WIDTH();
            weir_weir_data_geometry_hd.Level_Width.data = new List<List<float>>();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // Level_Width") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Data":
                        {
                            List<float> dataFloat = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                dataFloat.Add(float.Parse(s));
                            }
                            weir_weir_data_geometry_hd.Level_Width.data.Add(dataFloat);
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_STRUCTURE_MODULE_CROSSSECTIONS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[CROSSSECTIONS]") return false;
            femEngineHD.hydrodynamic_module.structure_module.crosssections = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.STRUCTURE_MODULE.CROSSSECTIONS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CROSSSECTIONS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "CrossSectionDataBridge":
                        femEngineHD.hydrodynamic_module.structure_module.crosssections.CrossSectionDataBridge = TheValue;
                        break;
                    case "CrossSectionFile":
                        femEngineHD.hydrodynamic_module.structure_module.crosssections.CrossSectionFile = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOURCES_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOURCES]")) return false;
            femEngineHD.hydrodynamic_module.sources = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[SOURCE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.sources.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.sources.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "number_of_sources":
                        femEngineHD.hydrodynamic_module.sources.number_of_sources = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOURCES_HD_SOURCE_HD(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // SOURCES")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOURCES_HD_SOURCE_HD(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string SourceStr = "";

            femEngineHD.hydrodynamic_module.sources.source = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE>();
            for (int i = 0; i < femEngineHD.hydrodynamic_module.sources.number_of_sources; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                SourceStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE source = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + SourceStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Name":
                            source.Name = TheValue;
                            break;
                        case "include":
                            source.include = int.Parse(TheValue);
                            break;
                        case "interpolation_type":
                            source.interpolation_type = int.Parse(TheValue);
                            break;
                        case "coordinate_type":
                            source.coordinate_type = TheValue;
                            break;
                        case "zone":
                            source.zone = int.Parse(TheValue);
                            break;
                        case "coordinates":
                            {
                                List<double> TempFloat = new List<double>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    TempFloat.Add(double.Parse(s));
                                }
                                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.COORDINATE coordinate =
                                    new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.COORDINATE();
                                coordinate.x = TempFloat[0];
                                coordinate.y = TempFloat[1];
                                coordinate.z = TempFloat[2];
                                source.coordinates = coordinate;
                            }
                            break;
                        case "layer":
                            source.layer = int.Parse(TheValue);
                            break;
                        case "distribution_type":
                            source.distribution_type = int.Parse(TheValue);
                            break;
                        case "connected_source":
                            source.connected_source = int.Parse(TheValue);
                            break;
                        case "type":
                            switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE)int.Parse(TheValue))
                            {
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.SimpleSource:
                                    source.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.SimpleSource;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.StandardSource:
                                    source.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.StandardSource;
                                    break;
                                case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.ConnectedSource:
                                    source.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOURCES.SOURCE.TYPE.ConnectedSource;
                                    break;
                                default:
                                    return false;
                            }
                            break;
                        case "format":
                            source.format = int.Parse(TheValue);
                            break;
                        case "file_name":
                            source.file_name = TheValue;
                            break;
                        case "constant_value":
                            source.constant_value = float.Parse(TheValue);
                            break;
                        case "constant_values":
                            {
                                source.constant_values = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    source.constant_values.Add(float.Parse(s));
                                }
                            }
                            break;
                        case "item_number":
                            source.item_number = int.Parse(TheValue);
                            break;
                        case "item_numbers":
                            {
                                source.item_numbers = new List<int>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    source.item_numbers.Add(int.Parse(s));
                                }
                            }
                            break;
                        case "item_name":
                            source.item_name = TheValue;
                            break;
                        case "item_names":
                            {
                                source.item_names = new List<string>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    source.item_names.Add(s);
                                }
                            }
                            source.item_name = TheValue;
                            break;
                        case "type_of_soft_start":
                            source.type_of_soft_start = int.Parse(TheValue);
                            break;
                        case "soft_time_interval":
                            source.soft_time_interval = int.Parse(TheValue);
                            break;
                        case "reference_value":
                            source.reference_value = int.Parse(TheValue);
                            break;
                        case "type_of_time_interpolation":
                            source.type_of_time_interpolation = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.hydrodynamic_module.sources.source.Add(SourceStr, source);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_RADIATION_STRESS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[RADIATION_STRESS]")) return false;
            femEngineHD.hydrodynamic_module.radiation_stress = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // RADIATION_STRESS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.radiation_stress.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS.TYPE.NoWaveRadiation:
                                femEngineHD.hydrodynamic_module.radiation_stress.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS.TYPE.NoWaveRadiation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS.TYPE.SpecifiedWaveRadiation:
                                femEngineHD.hydrodynamic_module.radiation_stress.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.RADIATION_STRESS.TYPE.SpecifiedWaveRadiation;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.radiation_stress.format = int.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.radiation_stress.file_name = TheValue;
                        break;
                    case "item_number_for_Sxx":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_number_for_Sxx = int.Parse(TheValue);
                        break;
                    case "item_number_for_Sxy":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_number_for_Sxy = int.Parse(TheValue);
                        break;
                    case "item_number_for_Syy":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_number_for_Syy = int.Parse(TheValue);
                        break;
                    case "item_name_for_Sxx":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_name_for_Sxx = TheValue;
                        break;
                    case "item_name_for_Sxy":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_name_for_Sxy = TheValue;
                        break;
                    case "item_name_for_Syy":
                        femEngineHD.hydrodynamic_module.radiation_stress.item_name_for_Syy = TheValue;
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.radiation_stress.soft_time_interval = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[PRECIPITATION_EVAPORATION]")) return false;
            femEngineHD.hydrodynamic_module.precipitation_evaporation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[PRECIPITATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "type_of_precipitation":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.NoPrecipitation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_precipitation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.NoPrecipitation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.SpecifiedPrecipitation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_precipitation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.SpecifiedPrecipitation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.NetPrecipitation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_precipitation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_PRECIPITATION.NetPrecipitation;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "type_of_evaporation":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.NoEvaporation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_evaporation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.NoEvaporation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.SpecifiedEvaporation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_evaporation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.SpecifiedEvaporation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.NetEvaporation:
                                femEngineHD.hydrodynamic_module.precipitation_evaporation.type_of_evaporation =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.TYPE_OF_EVAPORATION.NetEvaporation;
                                break;
                            default:
                                return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION_PRECIPITATION(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION_EVAPORATION(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // PRECIPITATION_EVAPORATION")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION_EVAPORATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[EVAPORATION]")) return false;
            femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.EVAPORATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EVAPORATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.evaporation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_PRECIPITATION_EVAPORATION_PRECIPITATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[PRECIPITATION]") return false;
            femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.PRECIPITATION_EVAPORATION.PRECIPITATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // PRECIPITATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.precipitation_evaporation.precipitation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIDAL_POTENTIAL(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[TIDAL_POTENTIAL]")) return false;
            femEngineHD.hydrodynamic_module.tidal_potential = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith(@"[CONSTITUENT_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.tidal_potential.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.hydrodynamic_module.tidal_potential.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.TYPE.TidalPotentialNotIncluded:
                                femEngineHD.hydrodynamic_module.tidal_potential.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.TYPE.TidalPotentialNotIncluded;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.TYPE.TidalPotentialIncluded:
                                femEngineHD.hydrodynamic_module.tidal_potential.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.TYPE.TidalPotentialIncluded;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.tidal_potential.format = int.Parse(TheValue);
                        break;
                    case "constituent_file_name":
                        femEngineHD.hydrodynamic_module.tidal_potential.constituent_file_name = TheValue;
                        break;
                    case "number_of_constituents":
                        femEngineHD.hydrodynamic_module.tidal_potential.number_of_constituents = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIDAL_POTENTIAL_CONSTITUENT(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // TIDAL_POTENTIAL")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIDAL_POTENTIAL_CONSTITUENT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string ConstituantStr = "";

            femEngineHD.hydrodynamic_module.tidal_potential.constituents = new Dictionary<string, m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.CONSTITUENT>();

            for (int i = 0; i < femEngineHD.hydrodynamic_module.tidal_potential.number_of_constituents; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                ConstituantStr = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.CONSTITUENT constituant = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIDAL_POTENTIAL.CONSTITUENT();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + ConstituantStr) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            constituant.Touched = int.Parse(TheValue);
                            break;
                        case "name":
                            constituant.name = TheValue;
                            break;
                        case "species":
                            constituant.species = int.Parse(TheValue);
                            break;
                        case "constituent":
                            constituant.constituent = int.Parse(TheValue);
                            break;
                        case "amplitude":
                            constituant.amplitude = float.Parse(TheValue);
                            break;
                        case "earthtide":
                            constituant.earthtide = float.Parse(TheValue);
                            break;
                        case "period_scaling":
                            constituant.period_scaling = int.Parse(TheValue);
                            break;
                        case "period":
                            constituant.period = double.Parse(TheValue);
                            break;
                        case "nodal_number_1":
                            constituant.nodal_number_1 = float.Parse(TheValue);
                            break;
                        case "nodal_number_2":
                            constituant.nodal_number_2 = float.Parse(TheValue);
                            break;
                        case "nodal_number_3":
                            constituant.nodal_number_3 = float.Parse(TheValue);
                            break;
                        case "arguments":
                            {
                                constituant.arguments = new List<float>();
                                foreach (string s in TheValue.Split(delimiter))
                                {
                                    constituant.arguments.Add(float.Parse(s));
                                }
                            }
                            break;
                        case "phase":
                            constituant.phase = int.Parse(TheValue);
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.hydrodynamic_module.tidal_potential.constituents.Add(ConstituantStr, constituant);

                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_ICE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[ICE]")) return false;
            femEngineHD.hydrodynamic_module.ice = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[ROUGHNESS]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.ice.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.NoIceCoverage:
                                femEngineHD.hydrodynamic_module.ice.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.NoIceCoverage;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceConcentration:
                                femEngineHD.hydrodynamic_module.ice.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceConcentration;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceThickness:
                                femEngineHD.hydrodynamic_module.ice.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceThickness;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceConcentrationAndThickness:
                                femEngineHD.hydrodynamic_module.ice.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.TYPE.SpecificIceConcentrationAndThickness;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.ice.format = int.Parse(TheValue);
                        break;
                    case "c_cut_off":
                        femEngineHD.hydrodynamic_module.ice.c_cut_off = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.ice.file_name = TheValue;
                        break;
                    case "item_number_concentration":
                        femEngineHD.hydrodynamic_module.ice.item_number_concentration = float.Parse(TheValue);
                        break;
                    case "item_number_thickness":
                        femEngineHD.hydrodynamic_module.ice.item_number_thickness = float.Parse(TheValue);
                        break;
                    case "item_name_concentration":
                        femEngineHD.hydrodynamic_module.ice.item_name_concentration = TheValue;
                        break;
                    case "item_name_thickness":
                        femEngineHD.hydrodynamic_module.ice.item_name_thickness = TheValue;
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_ICE_ROUGHNESS(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // ICE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_ICE_ROUGHNESS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[ROUGHNESS]") return false;
            femEngineHD.hydrodynamic_module.ice.roughness = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // ROUGHNESS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.ice.roughness.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS.TYPE.IceRoughnessHeightDataNotIncluded:
                                femEngineHD.hydrodynamic_module.ice.roughness.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS.TYPE.IceRoughnessHeightDataNotIncluded;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS.TYPE.IceRoughnessHeightDataIncluded:
                                femEngineHD.hydrodynamic_module.ice.roughness.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.ICE.ROUGHNESS.TYPE.IceRoughnessHeightDataIncluded;
                                break;
                            default:
                                return false;
                        }

                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.ice.roughness.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.ice.roughness.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.ice.roughness.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.ice.roughness.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.ice.roughness.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.ice.roughness.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.ice.roughness.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.ice.roughness.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.ice.roughness.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_WIND_FORCING(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[WIND_FORCING]")) return false;
            femEngineHD.hydrodynamic_module.wind_forcing = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[WIND_FRICTION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.TYPE.NotIncluded:
                                femEngineHD.hydrodynamic_module.wind_forcing.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.TYPE.NotIncluded;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.TYPE.Included:
                                femEngineHD.hydrodynamic_module.wind_forcing.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.TYPE.Included;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.wind_forcing.format = int.Parse(TheValue);
                        break;
                    case "constant_speed":
                        femEngineHD.hydrodynamic_module.wind_forcing.constant_speed = float.Parse(TheValue);
                        break;
                    case "constant_direction":
                        femEngineHD.hydrodynamic_module.wind_forcing.constant_direction = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.wind_forcing.file_name = TheValue;
                        break;
                    case "neutral_pressure":
                        femEngineHD.hydrodynamic_module.wind_forcing.neutral_pressure = float.Parse(TheValue);
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.wind_forcing.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.wind_forcing.soft_time_interval = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_WIND_FORCING_WIND_FRICTION(sr, femEngineHD, TheLine)) return false;
            if (!CheckNextLine(sr, @"EndSect  // WIND_FORCING")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_WIND_FORCING_WIND_FRICTION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[WIND_FRICTION]") return false;
            femEngineHD.hydrodynamic_module.wind_forcing.wind_friction = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // WIND_FRICTION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION.TYPE.Constant:
                                femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION.TYPE.Constant;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION.TYPE.VaryingWithWindSpeed:
                                femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.WIND_FORCING.WIND_FRICTION.TYPE.VaryingWithWindSpeed;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "constant_friction":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.constant_friction = float.Parse(TheValue);
                        break;
                    case "linear_friction_low":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.linear_friction_low = float.Parse(TheValue);
                        break;
                    case "linear_friction_high":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.linear_friction_high = float.Parse(TheValue);
                        break;
                    case "linear_speed_low":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.linear_speed_low = float.Parse(TheValue);
                        break;
                    case "linear_speed_high":
                        femEngineHD.hydrodynamic_module.wind_forcing.wind_friction.linear_speed_high = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_CORIOLIS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[CORIOLIS]")) return false;
            femEngineHD.hydrodynamic_module.coriolis = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CORIOLIS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.coriolis.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.NoCoriolisForce:
                                femEngineHD.hydrodynamic_module.coriolis.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.NoCoriolisForce;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.ConstantInDomain:
                                femEngineHD.hydrodynamic_module.coriolis.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.ConstantInDomain;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.VaryingInDomain:
                                femEngineHD.hydrodynamic_module.coriolis.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.CORIOLIS.TYPE.VaryingInDomain;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "latitude":
                        femEngineHD.hydrodynamic_module.coriolis.latitude = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[BED_RESISTANCE]")) return false;
            femEngineHD.hydrodynamic_module.bed_resistance = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[DRAG_COEFFICIENT]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.bed_resistance.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.NoBedResistance:
                                femEngineHD.hydrodynamic_module.bed_resistance.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.NoBedResistance;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.ChezyNumber:
                                femEngineHD.hydrodynamic_module.bed_resistance.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.ChezyNumber;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.ManningNumber:
                                femEngineHD.hydrodynamic_module.bed_resistance.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.ManningNumber;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.RoughnessHeight:
                                femEngineHD.hydrodynamic_module.bed_resistance.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.TYPE.RoughnessHeight;
                                break;
                            default:
                                return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_DRAG_COEFFICIENT(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_CHEZY_NUMBER(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_MANNING_NUMBER(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_ROUGHNESS(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // BED_RESISTANCE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_ROUGHNESS(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[ROUGHNESS]")) return false;
            femEngineHD.hydrodynamic_module.bed_resistance.roughness = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.ROUGHNESS();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // ROUGHNESS") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    case "type_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.type_of_Bottom_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.thickness_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "fraction_of_depth":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.fraction_of_depth = float.Parse(TheValue);
                        break;
                    case "fraction_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.roughness.fraction_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_MANNING_NUMBER(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[MANNING_NUMBER]")) return false;
            femEngineHD.hydrodynamic_module.bed_resistance.manning_number = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.MANNING_NUMBER();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // MANNING_NUMBER") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    case "type_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.type_of_Bottom_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.thickness_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "fraction_of_depth":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.fraction_of_depth = float.Parse(TheValue);
                        break;
                    case "fraction_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.bed_resistance.manning_number.fraction_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_CHEZY_NUMBER(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[CHEZY_NUMBER]")) return false;
            femEngineHD.hydrodynamic_module.bed_resistance.chezy_number = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.CHEZY_NUMBER();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CHEZY_NUMBER") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.bed_resistance.chezy_number.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_BED_RESISTANCE_DRAG_COEFFICIENT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[DRAG_COEFFICIENT]") return false;
            femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.BED_RESISTANCE.DRAG_COEFFICIENT();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DRAG_COEFFICIENT") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.bed_resistance.drag_coefficient.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[EDDY_VISCOSITY]")) return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[HORIZONTAL_EDDY_VISCOSITY]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.Touched = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // EDDY_VISCOSITY")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[VERTICAL_EDDY_VISCOSITY]")) return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.VERTICAL_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[CONSTANT_EDDY_FORMULATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.type = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_CONSTANT_EDDY_FORMULATION_VERT(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_LOG_LAW_FORMULATION(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_K_EPSILON_FORMULATION(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // VERTICAL_EDDY_VISCOSITY")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_K_EPSILON_FORMULATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[K_EPSILON_FORMULATION]")) return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.VERTICAL_EDDY_VISCOSITY.K_EPSILON_FORMULATION();
            int count = 0;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // K_EPSILON_FORMULATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.Touched = int.Parse(TheValue);
                        break;
                    case "type_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.type_of_Top_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.thickness_of_Top_layer = int.Parse(TheValue);
                        break;
                    case "fraction_of_depth":
                        {
                            if (count == 0)
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.fraction_of_depth = float.Parse(TheValue);
                            }
                            else
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.fraction_of_depth2 = float.Parse(TheValue);
                            }
                            count += 1;
                        }
                        break;
                    case "fraction_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.fraction_of_Top_layer = float.Parse(TheValue);
                        break;
                    case "type_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.type_of_Bottom_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.thickness_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "fraction_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.fraction_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "minimum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.minimum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    case "maximum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.k_epsilon_formulation.maximum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_LOG_LAW_FORMULATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[LOG_LAW_FORMULATION]")) return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.VERTICAL_EDDY_VISCOSITY.LOG_LAW_FORMULATION();
            int count = 0;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // LOG_LAW_FORMULATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.Touched = int.Parse(TheValue);
                        break;
                    case "type_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.type_of_Top_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.thickness_of_Top_layer = int.Parse(TheValue);
                        break;
                    case "fraction_of_depth":
                        {
                            if (count == 0)
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.fraction_of_depth = float.Parse(TheValue);
                            }
                            else
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.fraction_of_depth2 = float.Parse(TheValue);
                            }
                            count += 1;
                        }
                        break;
                    case "fraction_of_Top_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.fraction_of_Top_layer = float.Parse(TheValue);
                        break;
                    case "type_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.type_of_Bottom_layer = int.Parse(TheValue);
                        break;
                    case "thickness_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.thickness_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "fraction_of_Bottom_layer":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.fraction_of_Bottom_layer = float.Parse(TheValue);
                        break;
                    case "minimum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.minimum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    case "maximum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.maximum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    case "Ri_damping":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.Ri_damping = float.Parse(TheValue);
                        break;
                    case "Ri_a":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.Ri_a = float.Parse(TheValue);
                        break;
                    case "Ri_b":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.Ri_b = float.Parse(TheValue);
                        break;
                    case "mixing_length_constants":
                        {
                            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.mixing_length_constants = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.log_law_formulation.mixing_length_constants.Add(float.Parse(s));
                            }
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_VERTICAL_EDDY_VISCOSITY_CONSTANT_EDDY_FORMULATION_VERT(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[CONSTANT_EDDY_FORMULATION]") return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.VERTICAL_EDDY_VISCOSITY.CONSTANT_EDDY_FORMULATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CONSTANT_EDDY_FORMULATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.type = int.Parse(TheValue);
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    case "Ri_damping":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.Ri_damping = float.Parse(TheValue);
                        break;
                    case "Ri_a":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.Ri_a = float.Parse(TheValue);
                        break;
                    case "Ri_b":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.Ri_b = float.Parse(TheValue);
                        break;
                    case "mixing_length_constants":
                        {
                            femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.mixing_length_constants = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                femEngineHD.hydrodynamic_module.eddy_viscosity.vertical_eddy_viscosity.constant_eddy_formulation.mixing_length_constants.Add(float.Parse(s));
                            }
                        }
                        break;

                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[HORIZONTAL_EDDY_VISCOSITY]") return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[CONSTANT_EDDY_FORMULATION]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.NoEddy:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.NoEddy;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.ConstantEddyFormulation:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.ConstantEddyFormulation;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.SmagorinskyFormulation:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.TYPE.SmagorinskyFormulation;
                                break;
                            default:
                                return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY_CONSTANT_EDDY_FORMULATION_HOR(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY_SMAGORINSKY_FORMULATION(sr, femEngineHD)) return false;
            if (!CheckNextLine(sr, @"EndSect  // HORIZONTAL_EDDY_VISCOSITY")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY_SMAGORINSKY_FORMULATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SMAGORINSKY_FORMULATION]")) return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SMAGORINSKY_FORMULATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION.TYPE.Constant:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION.TYPE.Constant;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION.TYPE.VaryingInDomain:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.SMAGORINSKY_FORMULATION.TYPE.VaryingInDomain;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    case "minimum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.minimum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    case "maximum_eddy_viscosity":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.smagorinsky_formulation.maximum_eddy_viscosity = double.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EDDY_VISCOSITY_HORIZONTAL_EDDY_VISCOSITY_CONSTANT_EDDY_FORMULATION_HOR(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (TheLine != "[CONSTANT_EDDY_FORMULATION]") return false;
            femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation =
                new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.CONSTANT_EDDY_FORMULATION();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // CONSTANT_EDDY_FORMULATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.CONSTANT_EDDY_FORMULATION.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.CONSTANT_EDDY_FORMULATION.TYPE.Constant:
                                femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.type =
                                    m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EDDY_VISCOSITY.HORIZONTAL_EDDY_VISCOSITY.CONSTANT_EDDY_FORMULATION.TYPE.Constant;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "format":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.format = int.Parse(TheValue);
                        break;
                    case "constant_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.constant_value = float.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.file_name = TheValue;
                        break;
                    case "item_number":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.item_number = int.Parse(TheValue);
                        break;
                    case "item_name":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.item_name = TheValue;
                        break;
                    case "type_of_soft_start":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.type_of_soft_start = int.Parse(TheValue);
                        break;
                    case "soft_time_interval":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.soft_time_interval = int.Parse(TheValue);
                        break;
                    case "reference_value":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.reference_value = int.Parse(TheValue);
                        break;
                    case "type_of_time_interpolation":
                        femEngineHD.hydrodynamic_module.eddy_viscosity.horizontal_eddy_viscosity.constant_eddy_formulation.type_of_time_interpolation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DENSITY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DENSITY]")) return false;
            femEngineHD.hydrodynamic_module.density = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DENSITY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.density.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.Barotropic:
                                femEngineHD.hydrodynamic_module.density.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.Barotropic;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfTemperatureAndSalinity:
                                femEngineHD.hydrodynamic_module.density.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfTemperatureAndSalinity;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfTemperature:
                                femEngineHD.hydrodynamic_module.density.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfTemperature;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfSalinity:
                                femEngineHD.hydrodynamic_module.density.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DENSITY.TYPE.FunctionOfSalinity;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "temperature_reference":
                        femEngineHD.hydrodynamic_module.density.temperature_reference = float.Parse(TheValue);
                        break;
                    case "salinity_reference":
                        femEngineHD.hydrodynamic_module.density.salinity_reference = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DEPTH(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DEPTH]")) return false;
            femEngineHD.hydrodynamic_module.depth = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.DEPTH();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // DEPTH") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.depth.Touched = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_FLOOD_AND_DRY(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[FLOOD_AND_DRY]")) return false;
            femEngineHD.hydrodynamic_module.flood_and_dry = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // FLOOD_AND_DRY") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.flood_and_dry.Touched = int.Parse(TheValue);
                        break;
                    case "type":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY.TYPE)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY.TYPE.NotIncluded:
                                femEngineHD.hydrodynamic_module.flood_and_dry.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY.TYPE.NotIncluded;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY.TYPE.Included:
                                femEngineHD.hydrodynamic_module.flood_and_dry.type = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.FLOOD_AND_DRY.TYPE.Included;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "drying_depth":
                        femEngineHD.hydrodynamic_module.flood_and_dry.drying_depth = float.Parse(TheValue);
                        break;
                    case "flooding_depth":
                        femEngineHD.hydrodynamic_module.flood_and_dry.flooding_depth = float.Parse(TheValue);
                        break;
                    case "mass_depth":
                        femEngineHD.hydrodynamic_module.flood_and_dry.mass_depth = float.Parse(TheValue);
                        break;
                    case "depth":
                        femEngineHD.hydrodynamic_module.flood_and_dry.depth = float.Parse(TheValue);
                        break;
                    case "max_depth":
                        femEngineHD.hydrodynamic_module.flood_and_dry.max_depth = float.Parse(TheValue);
                        break;
                    case "width":
                        femEngineHD.hydrodynamic_module.flood_and_dry.width = float.Parse(TheValue);
                        break;
                    case "smoothing_parameter":
                        femEngineHD.hydrodynamic_module.flood_and_dry.smoothing_parameter = float.Parse(TheValue);
                        break;
                    case "friction_coefficient":
                        femEngineHD.hydrodynamic_module.flood_and_dry.friction_coefficient = float.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_LINEAR_EQUATION_SYSTEM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[LINEAR_EQUATION_SYSTEM]")) return false;
            femEngineHD.hydrodynamic_module.solution_technique.linear_equation_system = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.LINEAR_EQUATION_SYSTEM();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // LINEAR_EQUATION_SYSTEM") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "method":
                        femEngineHD.hydrodynamic_module.solution_technique.linear_equation_system.method = int.Parse(TheValue);
                        break;
                    case "tolerance":
                        femEngineHD.hydrodynamic_module.solution_technique.linear_equation_system.tolerance = float.Parse(TheValue);
                        break;
                    case "max_iterations":
                        femEngineHD.hydrodynamic_module.solution_technique.linear_equation_system.max_iterations = int.Parse(TheValue);
                        break;
                    case "type_of_print":
                        femEngineHD.hydrodynamic_module.solution_technique.linear_equation_system.type_of_print = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            if (TheLine != @"[DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM]") return false;
            femEngineHD.hydrodynamic_module.solution_technique.differential_algebraic_equation_system = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM();
            if (!CheckNextLine(sr, @"EndSect  // DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM")) return false;
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SOLUTION_TECHNIQUE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[SOLUTION_TECHNIQUE]")) return false;
            femEngineHD.hydrodynamic_module.solution_technique = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.hydrodynamic_module.solution_technique.Touched = int.Parse(TheValue);
                        break;
                    case "scheme_of_time_integration":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.LowOrder_FastAlgorithm:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_time_integration = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.LowOrder_FastAlgorithm;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.HighOrder:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_time_integration = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_TIME_INTEGRATION.HighOrder;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "scheme_of_space_discretization_horizontal":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL.LowOrder_FastAlgorithm:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_space_discretization_horizontal = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL.LowOrder_FastAlgorithm;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL.HighOrder:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_space_discretization_horizontal = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL.HighOrder;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "scheme_of_space_discretization_vertical":
                        switch ((m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL)int.Parse(TheValue))
                        {
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL.LowOrder_FastAlgorithm:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_space_discretization_vertical = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL.LowOrder_FastAlgorithm;
                                break;
                            case m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL.HighOrder:
                                femEngineHD.hydrodynamic_module.solution_technique.scheme_of_space_discretization_vertical = m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SOLUTION_TECHNIQUE.SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL.HighOrder;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case "method_of_space_discretization_horizontal":
                        femEngineHD.hydrodynamic_module.solution_technique.method_of_space_discretization_horizontal = int.Parse(TheValue);
                        break;
                    case "CFL_critical_HD":
                        femEngineHD.hydrodynamic_module.solution_technique.CFL_critical_HD = float.Parse(TheValue);
                        break;
                    case "dt_min_HD":
                        femEngineHD.hydrodynamic_module.solution_technique.dt_min_HD = float.Parse(TheValue);
                        break;
                    case "dt_max_HD":
                        femEngineHD.hydrodynamic_module.solution_technique.dt_max_HD = float.Parse(TheValue);
                        break;
                    case "CFL_critical_AD":
                        femEngineHD.hydrodynamic_module.solution_technique.CFL_critical_AD = float.Parse(TheValue);
                        break;
                    case "dt_min_AD":
                        femEngineHD.hydrodynamic_module.solution_technique.dt_min_AD = float.Parse(TheValue);
                        break;
                    case "dt_max_AD":
                        femEngineHD.hydrodynamic_module.solution_technique.dt_max_AD = float.Parse(TheValue);
                        break;
                    case "error_level":
                        femEngineHD.hydrodynamic_module.solution_technique.error_level = int.Parse(TheValue);
                        break;
                    case "maximum_number_of_errors":
                        femEngineHD.hydrodynamic_module.solution_technique.maximum_number_of_errors = int.Parse(TheValue);
                        break;
                    case "tau":
                        femEngineHD.hydrodynamic_module.solution_technique.tau = float.Parse(TheValue);
                        break;
                    case "theta":
                        femEngineHD.hydrodynamic_module.solution_technique.theta = float.Parse(TheValue);
                        break;
                    case "convection_bound":
                        femEngineHD.hydrodynamic_module.solution_technique.convection_bound = int.Parse(TheValue);
                        break;
                    case "artificial_diffusion":
                        femEngineHD.hydrodynamic_module.solution_technique.artificial_diffusion = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM(sr, femEngineHD, TheLine)) return false;
            if (!Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_LINEAR_EQUATION_SYSTEM(sr, femEngineHD)) return false;

            if (!CheckNextLine(sr, @"EndSect  // SOLUTION_TECHNIQUE")) return false;
            if (!CheckNextLine(sr, @"")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_SPACE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[SPACE]")) return false;
            femEngineHD.hydrodynamic_module.space = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.SPACE();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // SPACE") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "number_of_2D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.space.number_of_2D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_2D_mesh_velocity":
                        femEngineHD.hydrodynamic_module.space.number_of_2D_mesh_velocity = int.Parse(TheValue);
                        break;
                    case "number_of_2D_mesh_elevation":
                        femEngineHD.hydrodynamic_module.space.number_of_2D_mesh_elevation = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_geometry":
                        femEngineHD.hydrodynamic_module.space.number_of_3D_mesh_geometry = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_velocity":
                        femEngineHD.hydrodynamic_module.space.number_of_3D_mesh_velocity = int.Parse(TheValue);
                        break;
                    case "number_of_3D_mesh_pressure":
                        femEngineHD.hydrodynamic_module.space.number_of_3D_mesh_pressure = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_TIME(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[TIME]")) return false;
            femEngineHD.hydrodynamic_module.time = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.TIME();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TIME") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "start_time_step":
                        femEngineHD.hydrodynamic_module.time.start_time_step = int.Parse(TheValue);
                        break;
                    case "time_step_factor":
                        femEngineHD.hydrodynamic_module.time.time_step_factor = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_HYDRODYNAMIC_MODULE_EQUATION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            femEngineHD.hydrodynamic_module.equation = new m21_3fm.FemEngineHD.HYDRODYNAMIC_MODULE.EQUATION();
            while (true)
            {

                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // EQUATION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "formulation":
                        femEngineHD.hydrodynamic_module.equation.formulation = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_MODULE_SELECTION(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[MODULE_SELECTION]")) return false;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // MODULE_SELECTION") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.module_selection.Touched = int.Parse(TheValue);
                        break;
                    case "mode_of_hydrodynamic_module":
                        femEngineHD.module_selection.mode_of_hydrodynamic_module = int.Parse(TheValue);
                        break;
                    case "mode_of_spectral_wave_module":
                        femEngineHD.module_selection.mode_of_spectral_wave_module = int.Parse(TheValue);
                        break;
                    case "mode_of_transport_module":
                        femEngineHD.module_selection.mode_of_transport_module = int.Parse(TheValue);
                        break;
                    case "mode_of_mud_transport_module":
                        femEngineHD.module_selection.mode_of_mud_transport_module = int.Parse(TheValue);
                        break;
                    case "mode_of_eco_lab_module":
                        femEngineHD.module_selection.mode_of_eco_lab_module = int.Parse(TheValue);
                        break;
                    case "mode_of_sand_transport_module":
                        femEngineHD.module_selection.mode_of_sand_transport_module = int.Parse(TheValue);
                        break;
                    case "mode_of_particle_tracking_module":
                        femEngineHD.module_selection.mode_of_particle_tracking_module = int.Parse(TheValue);
                        break;
                    case "mode_of_oil_spill_module":
                        femEngineHD.module_selection.mode_of_oil_spill_module = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_TIME(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            if (!CheckNextLine(sr, "[TIME]")) return false;
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // TIME") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.time.Touched = int.Parse(TheValue);
                        break;
                    case "start_time":
                        {
                            int StartPos = 0;
                            int EndPos = TheValue.IndexOf(",", StartPos);
                            int Year = int.Parse(TheValue.Substring(StartPos, EndPos - StartPos));
                            StartPos = EndPos + 1;
                            EndPos = TheValue.IndexOf(",", StartPos);
                            int Month = int.Parse(TheValue.Substring(StartPos, EndPos - StartPos));
                            StartPos = EndPos + 1;
                            EndPos = TheValue.IndexOf(",", StartPos);
                            int Day = int.Parse(TheValue.Substring(StartPos, EndPos - StartPos));
                            StartPos = EndPos + 1;
                            EndPos = TheValue.IndexOf(",", StartPos);
                            int Hour = int.Parse(TheValue.Substring(StartPos, EndPos - StartPos));
                            StartPos = EndPos + 1;
                            EndPos = TheValue.IndexOf(",", StartPos);
                            int Minute = int.Parse(TheValue.Substring(StartPos, EndPos - StartPos));
                            StartPos = EndPos + 1;
                            int Second = int.Parse(TheValue.Substring(StartPos));

                            femEngineHD.time.start_time = new DateTime(Year, Month, Day, Hour, Minute, Second);
                        }
                        break;
                    case "time_step_interval":
                        femEngineHD.time.time_step_interval = int.Parse(TheValue);
                        break;
                    case "number_of_time_steps":
                        femEngineHD.time.number_of_time_steps = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_DOMAIN(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, "[DOMAIN]")) return false;

            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"[BOUNDARY_NAMES]") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.domain.Touched = int.Parse(TheValue);
                        break;
                    case "discretization":
                        femEngineHD.domain.discretization = int.Parse(TheValue);
                        break;
                    case "number_of_dimensions":
                        femEngineHD.domain.number_of_dimensions = int.Parse(TheValue);
                        break;
                    case "number_of_meshes":
                        femEngineHD.domain.number_of_meshes = int.Parse(TheValue);
                        break;
                    case "file_name":
                        femEngineHD.domain.file_name = TheValue;
                        break;
                    case "type_of_reordering":
                        femEngineHD.domain.type_of_reordering = int.Parse(TheValue);
                        break;
                    case "number_of_domains":
                        femEngineHD.domain.number_of_domains = int.Parse(TheValue);
                        break;
                    case "coordinate_type":
                        femEngineHD.domain.coordinate_type = TheValue;
                        break;
                    case "minimum_depth":
                        femEngineHD.domain.minimum_depth = double.Parse(TheValue);
                        break;
                    case "datum_depth":
                        femEngineHD.domain.datum_depth = float.Parse(TheValue);
                        break;
                    case "vertical_mesh_type_overall":
                        femEngineHD.domain.vertical_mesh_type_overall = int.Parse(TheValue);
                        break;
                    case "number_of_layers":
                        femEngineHD.domain.number_of_layers = int.Parse(TheValue);
                        break;
                    case "z_sigma":
                        femEngineHD.domain.z_sigma = double.Parse(TheValue);
                        break;
                    case "vertical_mesh_type":
                        femEngineHD.domain.vertical_mesh_type = int.Parse(TheValue);
                        break;
                    case "layer_thickness":
                        {
                            femEngineHD.domain.layer_thickness = new List<float>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                femEngineHD.domain.layer_thickness.Add(float.Parse(s));
                            }
                        }
                        break;
                    case "sigma_c":
                        femEngineHD.domain.sigma_c = float.Parse(TheValue);
                        break;
                    case "theta":
                        femEngineHD.domain.theta = int.Parse(TheValue);
                        break;
                    case "b":
                        femEngineHD.domain.b = int.Parse(TheValue);
                        break;
                    case "number_of_layers_zlevel":
                        femEngineHD.domain.number_of_layers_zlevel = int.Parse(TheValue);
                        break;
                    case "vertical_mesh_type_zlevel":
                        femEngineHD.domain.vertical_mesh_type_zlevel = int.Parse(TheValue);
                        break;
                    case "constant_layer_thickness_zlevel":
                        femEngineHD.domain.constant_layer_thickness_zlevel = double.Parse(TheValue);
                        break;
                    case "variable_layer_thickness_zlevel":
                        {
                            femEngineHD.domain.variable_layer_thickness_zlevel = new List<double>();
                            foreach (string s in TheValue.Split(delimiter))
                            {
                                femEngineHD.domain.variable_layer_thickness_zlevel.Add(double.Parse(s));
                            }
                        }
                        break;
                    case "type_of_bathymetry_adjustment":
                        femEngineHD.domain.type_of_bathymetry_adjustment = int.Parse(TheValue);
                        break;
                    case "minimum_layer_thickness_zlevel":
                        femEngineHD.domain.minimum_layer_thickness_zlevel = double.Parse(TheValue);
                        break;
                    case "type_of_mesh":
                        femEngineHD.domain.type_of_mesh = int.Parse(TheValue);
                        break;
                    case "type_of_gauss":
                        femEngineHD.domain.type_of_gauss = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_DOMAIN_BOUNDARY_NAMES(sr, femEngineHD)) return false;
            if (!Read_m21_3fm_FemEngineHD_DOMAIN_GIS_BACKGROUND(sr, femEngineHD)) return false;

            if (!CheckNextLine(sr, @"EndSect  // DOMAIN")) return false;
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_DOMAIN_GIS_BACKGROUND(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            if (!CheckNextLine(sr, @"[GIS_BACKGROUND]")) return false;

            m21_3fm.FemEngineHD.DOMAIN.GIS_BACKGROUND gis_background = new m21_3fm.FemEngineHD.DOMAIN.GIS_BACKGROUND();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine == @"EndSect  // GIS_BACKGROUND") break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        gis_background.Touched = int.Parse(TheValue);
                        break;
                    case "file_name":
                        gis_background.file_Name = TheValue;
                        break;
                    default:
                        return false;
                }
            }

            femEngineHD.domain.gis_background = gis_background;

            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_DOMAIN_BOUNDARY_NAMES(StreamReader sr, m21_3fm.FemEngineHD femEngineHD)
        {
            RaiseMessageEvent("");
            string TheLine = "";
            string VariableName = "";
            string TheValue = "";

            //if (!CheckNextLine(sr, "[BOUNDARY_NAMES]")) return false;

            femEngineHD.domain.boundary_names = new m21_3fm.FemEngineHD.DOMAIN.BOUNDARY_NAMES();
            while (true)
            {
                TheLine = GetTheLine(sr);
                if (TheLine.StartsWith("[CODE_")) break;
                VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                switch (VariableName)
                {
                    case "Touched":
                        femEngineHD.domain.boundary_names.Touched = int.Parse(TheValue);
                        break;
                    case "MzSEPfsListItemCount":
                        femEngineHD.domain.boundary_names.MzSEPfsListItemCount = int.Parse(TheValue);
                        break;
                    default:
                        return false;
                }
            }
            if (!Read_m21_3fm_FemEngineHD_DOMAIN_BOUNDARY_NAMES_CODE(sr, femEngineHD, TheLine)) return false;

            if (!CheckNextLine(sr, @"EndSect  // BOUNDARY_NAMES")) return false;
            if (!CheckNextLine(sr, "")) return false;
            return true;
        }
        private bool Read_m21_3fm_FemEngineHD_DOMAIN_BOUNDARY_NAMES_CODE(StreamReader sr, m21_3fm.FemEngineHD femEngineHD, string TheLine)
        {
            RaiseMessageEvent("");
            //string TheLine = "";
            string VariableName = "";
            string TheValue = "";
            string BN_Code = "";

            femEngineHD.domain.boundary_names.boundary_code = new Dictionary<string, m21_3fm.FemEngineHD.DOMAIN.BOUNDARY_NAMES.BOUNDARY_CODE>();
            if (!TheLine.StartsWith("[CODE_")) return false;
            for (int i = 0; i < femEngineHD.domain.boundary_names.MzSEPfsListItemCount; i++)
            {
                if (i != 0)
                {
                    TheLine = GetTheLine(sr);
                }
                BN_Code = TheLine.Substring(1, TheLine.Length - 2);
                m21_3fm.FemEngineHD.DOMAIN.BOUNDARY_NAMES.BOUNDARY_CODE boundary_Code = new m21_3fm.FemEngineHD.DOMAIN.BOUNDARY_NAMES.BOUNDARY_CODE();
                while (true)
                {
                    TheLine = GetTheLine(sr);
                    if (TheLine == @"EndSect  // " + BN_Code) break;
                    VariableName = TheLine.Substring(0, TheLine.IndexOf("=") - 1).Trim();
                    TheValue = TheLine.Substring(TheLine.IndexOf("=") + 1).Trim();
                    switch (VariableName)
                    {
                        case "Touched":
                            boundary_Code.Touched = int.Parse(TheValue);
                            break;
                        case "name":
                            boundary_Code.Name = TheValue;
                            break;
                        default:
                            return false;
                    }
                }
                femEngineHD.domain.boundary_names.boundary_code.Add(BN_Code, boundary_Code);

                //if (!CheckNextLine(sr, @"EndSect  // " + BN_Code)) return false;
                if (!CheckNextLine(sr, "")) return false;
            }
            return true;
        }
    }
}
