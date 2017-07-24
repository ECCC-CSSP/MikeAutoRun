using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;

namespace MikeAutoRun
{
    public partial class m21_3fm
    {
        // Variables
        private FileInfo fi = new FileInfo(@"C:\");

        // Properties
        public TopFileInfo topfileinfo { get; set; }
        public FemEngineHD femEngineHD { get; set; }
        public SYSTEM system { get; set; }
        public StringBuilder sbFileTxt { get; set; }

        // Event
        public event ErrorEventHandler m21_3fmError;


        // Miscellaneous messages event ex: beginning file read
        #region Message Event
        // event attributes
        public class m21_3fmMessageEventArgs : EventArgs
        {
            // Properties
            public string Message { get; set; }

            // Constructor
            public m21_3fmMessageEventArgs(string Message)
            {
                this.Message = Message;
            }
        }
        // delegate function attached to in parent object
        public delegate void M21_3fmMessageEventHandler(object sender, m21_3fmMessageEventArgs e);
        // event function called from within m21_3fm
        public event M21_3fmMessageEventHandler m21_3fmMessage;
        private void RaiseMessageEvent(string Message)
        {
            if (this.m21_3fmMessage != null)
                m21_3fmMessage(this, new m21_3fmMessageEventArgs(Message));
        }

        #endregion

        // Constructor 
        public m21_3fm()
        {
            topfileinfo = new TopFileInfo();
            femEngineHD = new FemEngineHD();
            system = new SYSTEM();
            sbFileTxt = new StringBuilder();
        }

        public bool Read_M21_3FM_File(string FullFileName)
        {
            string OldCultureName = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-CA");

            fi = new FileInfo(FullFileName);

            if (!fi.Exists)
            {
                m21_3fmError(this, new ErrorEventArgs(new Exception("File [" + fi.FullName + "] does not exist.")));
                Thread.CurrentThread.CurrentCulture = new CultureInfo(OldCultureName);
                return false;
            }

            StreamReader sr = fi.OpenText();

            if (!Read_m21_3fm_Document(sr, femEngineHD))
            {
                RaiseMessageEvent(this.sbFileTxt.ToString());
                RaiseMessageEvent("Error occured while reading m21fm or m3fm file ...\r\n\r\n");
                sr.Close();
                Thread.CurrentThread.CurrentCulture = new CultureInfo(OldCultureName);
                return false;
            }
            else
            {
                RaiseMessageEvent("Read completed ...\r\n\r\n");
            }

            try
            {
                sr.Close();
            }
            catch (Exception)
            {
                // nothing for now
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(OldCultureName);

            return true;
        }
        //public bool Write_M21_3FM_File(string FullFileName, m21_3fm m21_3fmInput)
        //{
        //    fi = new FileInfo(FullFileName);

        //    StreamWriter sw = fi.CreateText();

        //    if (!Write_M21_3FM_Document(sw, m21_3fmInput))
        //    {
        //        RaiseMessageEvent(this.sbFileTxt.ToString());
        //        RaiseMessageEvent("Error occured while reading m21fm or m3fm file ...\r\n\r\n");
        //        sw.Close();
        //        return false;
        //    }
        //    else
        //    {
        //        RaiseMessageEvent("Read completed ...\r\n\r\n");
        //    }
        //    try
        //    {
        //        sw.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        // nothing for now
        //    }

        //    return true;
        //}

        public class TopFileInfo
        {
            public DateTime Created { get; set; }
            public string DLLid { get; set; }
            public string PFSversion { get; set; }
        }
        public class FemEngineHD
        {
            public DOMAIN domain { get; set; }
            public TIME time { get; set; }
            public MODULE_SELECTION module_selection { get; set; }
            public HYDRODYNAMIC_MODULE hydrodynamic_module { get; set; }
            public TRANSPORT_MODULE transport_module { get; set; }
            public string ecolab_module { get; set; }
            public string mud_transport_module { get; set; }
            public string sand_transport_module { get; set; }
            public string particle_tracking_module { get; set; }

            public class DOMAIN
            {
                public int Touched { get; set; }
                public int discretization { get; set; }
                public int number_of_dimensions { get; set; }
                public int number_of_meshes { get; set; }
                public string file_name { get; set; }
                public int type_of_reordering { get; set; }
                public int number_of_domains { get; set; }
                public string coordinate_type { get; set; }
                public double minimum_depth { get; set; }
                public float datum_depth { get; set; }
                public int vertical_mesh_type_overall { get; set; }
                public int number_of_layers { get; set; }
                public double z_sigma { get; set; }
                public int vertical_mesh_type { get; set; }
                public List<float> layer_thickness { get; set; }
                public float sigma_c { get; set; }
                public int theta { get; set; }
                public int b { get; set; }
                public int number_of_layers_zlevel { get; set; }
                public int vertical_mesh_type_zlevel { get; set; }
                public double constant_layer_thickness_zlevel { get; set; }
                public List<double> variable_layer_thickness_zlevel { get; set; }
                public int type_of_bathymetry_adjustment { get; set; }
                public double minimum_layer_thickness_zlevel { get; set; }
                public int type_of_mesh { get; set; }
                public int type_of_gauss { get; set; }
                public BOUNDARY_NAMES boundary_names { get; set; }
                public GIS_BACKGROUND gis_background { get; set; }

                public class BOUNDARY_NAMES
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, BOUNDARY_CODE> boundary_code { get; set; }

                    public class BOUNDARY_CODE
                    {
                        public int Touched { get; set; }
                        public string Name { get; set; }
                    }
                }
                public class GIS_BACKGROUND
                {
                    public int Touched { get; set; }
                    public string file_Name { get; set; }
                }
            }

            public class TIME
            {
                public int Touched { get; set; }
                public DateTime start_time { get; set; }
                public int time_step_interval { get; set; }
                public int number_of_time_steps { get; set; }
            }

            public class MODULE_SELECTION
            {
                public int Touched { get; set; }
                public int mode_of_hydrodynamic_module { get; set; }
                public int mode_of_spectral_wave_module { get; set; }
                public int mode_of_transport_module { get; set; }
                public int mode_of_mud_transport_module { get; set; }
                public int mode_of_eco_lab_module { get; set; }
                public int mode_of_sand_transport_module { get; set; }
                public int mode_of_particle_tracking_module { get; set; }
                public int mode_of_oil_spill_module { get; set; }
            }

            public class HYDRODYNAMIC_MODULE
            {
                public int mode { get; set; }
                public EQUATION equation { get; set; }
                public TIME time { get; set; }
                public SPACE space { get; set; }
                public SOLUTION_TECHNIQUE solution_technique { get; set; }
                public FLOOD_AND_DRY flood_and_dry { get; set; }
                public DEPTH depth { get; set; }
                public DENSITY density { get; set; }
                public EDDY_VISCOSITY eddy_viscosity { get; set; }
                public BED_RESISTANCE bed_resistance { get; set; }
                public CORIOLIS coriolis { get; set; }
                public WIND_FORCING wind_forcing { get; set; }
                public ICE ice { get; set; }
                public TIDAL_POTENTIAL tidal_potential { get; set; }
                public PRECIPITATION_EVAPORATION precipitation_evaporation { get; set; }
                public RADIATION_STRESS radiation_stress { get; set; }
                public SOURCES sources { get; set; }
                public STRUCTURE_MODULE structure_module { get; set; }
                public STRUCTURES structures { get; set; }
                public INITIAL_CONDITIONS initial_conditions { get; set; }
                public BOUNDARY_CONDITIONS boundary_conditions { get; set; }
                public TEMPERATURE_SALINITY_MODULE temperature_salinity_module { get; set; }
                public TURBULENCE_MODULE turbulence_module { get; set; }
                public DECOUPLING decoupling { get; set; }
                public OUTPUTS outputs { get; set; }

                public class EQUATION
                {
                    public int formulation { get; set; }
                }
                public class TIME
                {
                    public int start_time_step { get; set; }
                    public int time_step_factor { get; set; }
                }
                public class SPACE
                {
                    public int number_of_2D_mesh_geometry { get; set; }
                    public int number_of_2D_mesh_velocity { get; set; }
                    public int number_of_2D_mesh_elevation { get; set; }
                    public int number_of_3D_mesh_geometry { get; set; }
                    public int number_of_3D_mesh_velocity { get; set; }
                    public int number_of_3D_mesh_pressure { get; set; }
                }
                public class SOLUTION_TECHNIQUE
                {
                    public int Touched { get; set; }
                    public SCHEME_OF_TIME_INTEGRATION scheme_of_time_integration { get; set; }
                    public SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL scheme_of_space_discretization_horizontal { get; set; }
                    public SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL scheme_of_space_discretization_vertical { get; set; }
                    public int method_of_space_discretization_horizontal { get; set; }
                    public float CFL_critical_HD { get; set; }
                    public float dt_min_HD { get; set; }
                    public float dt_max_HD { get; set; }
                    public float CFL_critical_AD { get; set; }
                    public float dt_min_AD { get; set; }
                    public float dt_max_AD { get; set; }
                    public int error_level { get; set; }
                    public int maximum_number_of_errors { get; set; }
                    public float tau { get; set; }
                    public float theta { get; set; }
                    public int convection_bound { get; set; }
                    public int artificial_diffusion { get; set; }
                    public DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM differential_algebraic_equation_system { get; set; }
                    public LINEAR_EQUATION_SYSTEM linear_equation_system { get; set; }

                    public enum SCHEME_OF_TIME_INTEGRATION
                    {
                        LowOrder_FastAlgorithm = 1,
                        HighOrder = 2
                    }
                    public enum SCHEME_OF_SPACE_DISCRETIZATION_HORIZONTAL
                    {
                        LowOrder_FastAlgorithm = 1,
                        HighOrder = 2
                    }
                    public enum SCHEME_OF_SPACE_DISCRETIZATION_VERTICAL
                    {
                        LowOrder_FastAlgorithm = 1,
                        HighOrder = 3
                    }
                    public class DIFFERENTIAL_ALGEBRAIC_EQUATION_SYSTEM
                    {
                    }
                    public class LINEAR_EQUATION_SYSTEM
                    {
                        public int method { get; set; }
                        public float tolerance { get; set; }
                        public int max_iterations { get; set; }
                        public int type_of_print { get; set; }
                    }
                }
                public class FLOOD_AND_DRY
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public float drying_depth { get; set; }
                    public float flooding_depth { get; set; }
                    public float mass_depth { get; set; }
                    public float depth { get; set; }
                    public float max_depth { get; set; }
                    public float width { get; set; }
                    public float smoothing_parameter { get; set; }
                    public float friction_coefficient { get; set; }
                    public enum TYPE
                    {
                        NotIncluded = 0,
                        Included = 2
                    }
                }
                public class DEPTH
                {
                    public int Touched { get; set; }
                }
                public class DENSITY
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public float temperature_reference { get; set; }
                    public float salinity_reference { get; set; }
                    public enum TYPE
                    {
                        Barotropic = 0,
                        FunctionOfTemperatureAndSalinity = 1,
                        FunctionOfTemperature = 2,
                        FunctionOfSalinity = 3
                    }
                }
                public class EDDY_VISCOSITY
                {
                    public int Touched { get; set; }
                    public HORIZONTAL_EDDY_VISCOSITY horizontal_eddy_viscosity { get; set; }
                    public VERTICAL_EDDY_VISCOSITY vertical_eddy_viscosity { get; set; }

                    public class HORIZONTAL_EDDY_VISCOSITY
                    {
                        public int Touched { get; set; }
                        public TYPE type { get; set; }
                        public CONSTANT_EDDY_FORMULATION constant_eddy_formulation { get; set; }
                        public SMAGORINSKY_FORMULATION smagorinsky_formulation { get; set; }

                        public enum TYPE
                        {
                            NoEddy = 0,
                            ConstantEddyFormulation = 1,
                            SmagorinskyFormulation = 3
                        }
                        public class CONSTANT_EDDY_FORMULATION
                        {
                            public int Touched { get; set; }
                            public TYPE type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }

                            public enum TYPE
                            {
                                Constant = 1,
                            }
                        }
                        public class SMAGORINSKY_FORMULATION
                        {
                            public int Touched { get; set; }
                            public TYPE type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                            public double minimum_eddy_viscosity { get; set; }
                            public double maximum_eddy_viscosity { get; set; }

                            public enum TYPE
                            {
                                Constant = 1,
                                VaryingInDomain = 3
                            }
                        }

                    }
                    public class VERTICAL_EDDY_VISCOSITY
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public CONSTANT_EDDY_FORMULATION constant_eddy_formulation { get; set; }
                        public LOG_LAW_FORMULATION log_law_formulation { get; set; }
                        public K_EPSILON_FORMULATION k_epsilon_formulation { get; set; }

                        public class CONSTANT_EDDY_FORMULATION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                            public float Ri_damping { get; set; }
                            public float Ri_a { get; set; }
                            public float Ri_b { get; set; }
                            public List<float> mixing_length_constants { get; set; }
                        }
                        public class LOG_LAW_FORMULATION
                        {
                            public int Touched { get; set; }
                            public int type_of_Top_layer { get; set; }
                            public int thickness_of_Top_layer { get; set; }
                            public float fraction_of_depth { get; set; }
                            public float fraction_of_Top_layer { get; set; }
                            public int type_of_Bottom_layer { get; set; }
                            public float thickness_of_Bottom_layer { get; set; }
                            public float fraction_of_depth2 { get; set; }
                            public float fraction_of_Bottom_layer { get; set; }
                            public double minimum_eddy_viscosity { get; set; }
                            public double maximum_eddy_viscosity { get; set; }
                            public double Ri_damping { get; set; }
                            public float Ri_a { get; set; }
                            public float Ri_b { get; set; }
                            public List<float> mixing_length_constants { get; set; }
                        }
                        public class K_EPSILON_FORMULATION
                        {
                            public int Touched { get; set; }
                            public int type_of_Top_layer { get; set; }
                            public int thickness_of_Top_layer { get; set; }
                            public float fraction_of_depth { get; set; }
                            public float fraction_of_Top_layer { get; set; }
                            public int type_of_Bottom_layer { get; set; }
                            public float thickness_of_Bottom_layer { get; set; }
                            public float fraction_of_depth2 { get; set; }
                            public float fraction_of_Bottom_layer { get; set; }
                            public double minimum_eddy_viscosity { get; set; }
                            public double maximum_eddy_viscosity { get; set; }
                        }
                    }
                }
                public class BED_RESISTANCE
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public DRAG_COEFFICIENT drag_coefficient { get; set; }
                    public CHEZY_NUMBER chezy_number { get; set; }
                    public MANNING_NUMBER manning_number { get; set; }
                    public ROUGHNESS roughness { get; set; }

                    public enum TYPE
                    {
                        NoBedResistance = 0,
                        ChezyNumber = 3,
                        ManningNumber = 4,
                        RoughnessHeight = 5
                    }
                    public class DRAG_COEFFICIENT
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                    public class CHEZY_NUMBER
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                    public class MANNING_NUMBER
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                        public int type_of_Bottom_layer { get; set; }
                        public float thickness_of_Bottom_layer { get; set; }
                        public float fraction_of_depth { get; set; }
                        public float fraction_of_Bottom_layer { get; set; }
                    }
                    public class ROUGHNESS
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                        public int type_of_Bottom_layer { get; set; }
                        public float thickness_of_Bottom_layer { get; set; }
                        public float fraction_of_depth { get; set; }
                        public float fraction_of_Bottom_layer { get; set; }
                    }
                }
                public class CORIOLIS
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public int latitude { get; set; }

                    public enum TYPE
                    {
                        NoCoriolisForce = 0,
                        ConstantInDomain = 1,
                        VaryingInDomain = 2
                    }
                }
                public class WIND_FORCING
                {
                    public TYPE type { get; set; }
                    public int format { get; set; }
                    public float constant_speed { get; set; }
                    public float constant_direction { get; set; }
                    public string file_name { get; set; }
                    public float neutral_pressure { get; set; }
                    public int type_of_soft_start { get; set; }
                    public int soft_time_interval { get; set; }
                    public WIND_FRICTION wind_friction { get; set; }

                    public enum TYPE
                    {
                        NotIncluded = 0,
                        Included = 1
                    }
                    public class WIND_FRICTION
                    {
                        public int Touched { get; set; }
                        public TYPE type { get; set; }
                        public float constant_friction { get; set; }
                        public float linear_friction_low { get; set; }
                        public float linear_friction_high { get; set; }
                        public float linear_speed_low { get; set; }
                        public float linear_speed_high { get; set; }

                        public enum TYPE
                        {
                            Constant = 0,
                            VaryingWithWindSpeed = 1
                        }
                    }
                }
                public class ICE
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public int format { get; set; }
                    public float c_cut_off { get; set; }
                    public string file_name { get; set; }
                    public float item_number_concentration { get; set; }
                    public float item_number_thickness { get; set; }
                    public string item_name_concentration { get; set; }
                    public string item_name_thickness { get; set; }
                    public ROUGHNESS roughness { get; set; }

                    public enum TYPE
                    {
                        NoIceCoverage = 0,
                        SpecificIceConcentration = 1,
                        SpecificIceThickness = 2,
                        SpecificIceConcentrationAndThickness = 4
                    }
                    public class ROUGHNESS
                    {
                        public int Touched { get; set; }
                        public TYPE type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }

                        public enum TYPE
                        {
                            IceRoughnessHeightDataNotIncluded = 0,
                            IceRoughnessHeightDataIncluded = 1,
                        }
                    }
                }
                public class TIDAL_POTENTIAL
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public TYPE type { get; set; }
                    public int format { get; set; }
                    public string constituent_file_name { get; set; }
                    public int number_of_constituents { get; set; }
                    public Dictionary<string, CONSTITUENT> constituents { get; set; }

                    public enum TYPE
                    {
                        TidalPotentialNotIncluded = 0,
                        TidalPotentialIncluded = 1
                    }
                    public class CONSTITUENT
                    {
                        public int Touched { get; set; }
                        public string name { get; set; }
                        public int species { get; set; }
                        public int constituent { get; set; }
                        public float amplitude { get; set; }
                        public float earthtide { get; set; }
                        public int period_scaling { get; set; }
                        public double period { get; set; }
                        public float nodal_number_1 { get; set; }
                        public float nodal_number_2 { get; set; }
                        public float nodal_number_3 { get; set; }
                        public List<float> arguments { get; set; }
                        public int phase { get; set; }
                    }
                }
                public class PRECIPITATION_EVAPORATION
                {
                    public int Touched { get; set; }
                    public TYPE_OF_PRECIPITATION type_of_precipitation { get; set; }
                    public TYPE_OF_EVAPORATION type_of_evaporation { get; set; }
                    public PRECIPITATION precipitation { get; set; }
                    public EVAPORATION evaporation { get; set; }

                    public enum TYPE_OF_PRECIPITATION
                    {
                        NoPrecipitation = 0,
                        SpecifiedPrecipitation = 1,
                        NetPrecipitation = 2
                    }
                    public enum TYPE_OF_EVAPORATION
                    {
                        NoEvaporation = 0,
                        SpecifiedEvaporation = 1,
                        NetEvaporation = 2
                    }
                    public class PRECIPITATION
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                    public class EVAPORATION
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                }
                public class RADIATION_STRESS
                {
                    public int Touched { get; set; }
                    public TYPE type { get; set; }
                    public int format { get; set; }
                    public string file_name { get; set; }
                    public int item_number_for_Sxx { get; set; }
                    public int item_number_for_Sxy { get; set; }
                    public int item_number_for_Syy { get; set; }
                    public string item_name_for_Sxx { get; set; }
                    public string item_name_for_Sxy { get; set; }
                    public string item_name_for_Syy { get; set; }
                    public int soft_time_interval { get; set; }

                    public enum TYPE
                    {
                        NoWaveRadiation = 0,
                        SpecifiedWaveRadiation = 1
                    }
                }
                public class SOURCES
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public int number_of_sources { get; set; }
                    public Dictionary<string, SOURCE> source { get; set; }

                    public class SOURCE
                    {
                        public string Name { get; set; }
                        public int include { get; set; }
                        public int interpolation_type { get; set; }
                        public string coordinate_type { get; set; }
                        public int zone { get; set; }
                        public COORDINATE coordinates { get; set; }
                        public int layer { get; set; }
                        public int distribution_type { get; set; }
                        public int connected_source { get; set; }
                        public TYPE type { get; set; }
                        public int format { get; set; }
                        public string file_name { get; set; }
                        public float constant_value { get; set; }
                        public List<float> constant_values { get; set; }
                        public int item_number { get; set; }
                        public List<int> item_numbers { get; set; }
                        public string item_name { get; set; }
                        public List<string> item_names { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }

                        public class COORDINATE
                        {
                            public double x { get; set; }
                            public double y { get; set; }
                            public double z { get; set; }
                        }
                        public enum TYPE
                        {
                            SimpleSource = 0,
                            StandardSource = 1,
                            ConnectedSource = 3
                        }
                    }
                }
                public class STRUCTURE_MODULE
                {
                    public List<int> Structure_Version { get; set; }
                    public CROSSSECTIONS crosssections { get; set; }
                    public WEIR weir { get; set; }
                    public CULVERTS culverts { get; set; }

                    public class CROSSSECTIONS
                    {
                        public string CrossSectionDataBridge { get; set; }
                        public string CrossSectionFile { get; set; }
                    }
                    public class WEIR
                    {
                        public List<WEIR_DATA> weir_data { get; set; }

                        public class WEIR_DATA
                        {
                            public List<string> Location { get; set; }
                            public float delhs { get; set; }
                            public string coordinate_type { get; set; }
                            public int number_of_points { get; set; }
                            public List<double> x { get; set; }
                            public List<double> y { get; set; }
                            public float HorizOffset { get; set; }
                            public ATTRIBUTES attributes { get; set; }
                            public List<float> HeadLossFactors { get; set; }
                            public List<float> WeirFormulaParam { get; set; }
                            public List<float> WeirFormula2Param { get; set; }
                            public List<float> WeirFormula3Param { get; set; }
                            public GEOMETRY Geometry { get; set; }

                            public class ATTRIBUTES
                            {
                                public TYPE type { get; set; }
                                public VALVE valve { get; set; }

                                public enum TYPE
                                {
                                    BroadCrestedWeir = 0,
                                    WeirFormula1 = 2,
                                    WeirFormula2Honma = 3
                                }
                                public enum VALVE
                                {
                                    None = 0,
                                    OnlyNegativeFlow = 1,
                                    OnlyPositiveFlow = 2,
                                    NoFlow = 3
                                }
                            }
                            public class GEOMETRY
                            {
                                public List<int> Attributes { get; set; }
                                public LEVEL_WIDTH Level_Width { get; set; }
                                public class LEVEL_WIDTH
                                {
                                    public List<List<float>> data { get; set; }
                                }
                            }
                        }
                    }
                    public class CULVERTS
                    {
                        public List<CULVERT_DATA> culvert_data { get; set; }

                        public class CULVERT_DATA
                        {
                            public List<string> Location { get; set; }
                            public float delhs { get; set; }
                            public string coordinate_type { get; set; }
                            public int number_of_points { get; set; }
                            public List<double> x { get; set; }
                            public List<double> y { get; set; }
                            public float HorizOffset { get; set; }
                            public ATTRIBUTES attributes { get; set; }
                            public GEOMETRY geometry { get; set; }
                            public List<float> HeadLossFactors { get; set; }

                            public class ATTRIBUTES
                            {
                                public float Upstream { get; set; }
                                public float Downstream { get; set; }
                                public float Length { get; set; }
                                public float Manning_n { get; set; }
                                public int NumberOfCulverts { get; set; }
                                public VALVE_REGULATION valve_regulation { get; set; }
                                public SECTION_TYPE section_type { get; set; }

                                public enum VALVE_REGULATION
                                {
                                    None = 0,
                                    OnlyNegativeFlow = 1,
                                    OnlyPositiveFlow = 2,
                                    NoFlow = 3
                                }
                                public enum SECTION_TYPE
                                {
                                    Closed = 0,
                                    Open = 1
                                }
                            }
                            public class GEOMETRY
                            {
                                public TYPE Type { get; set; }
                                public List<float> Rectangular { get; set; }
                                public float Cicular_Diameter { get; set; }
                                public IRREGULAR culverts_hd_culvert_data_hd_geometry_hd_irregular_hd { get; set; }

                                public enum TYPE
                                {
                                    Rectangular = 0,
                                    Circular = 1,
                                    IrregularLevelWidthTable = 2
                                }
                                public class IRREGULAR
                                {
                                    public List<DATA> data { get; set; }

                                    public class DATA
                                    {
                                        public List<float> data { get; set; }
                                    }
                                }
                            }
                        }
                    }
                }
                public class STRUCTURES
                {
                    public GATES gates { get; set; }
                    public PIERS piers { get; set; }
                    public TURBINES turbines { get; set; }

                    public class GATES
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public int number_of_gates { get; set; }
                        public Dictionary<string, GATE> gate { get; set; }

                        public class GATE
                        {
                            public string Name { get; set; }
                            public int include { get; set; }
                            public int input_format { get; set; }
                            public string coordinate_type { get; set; }
                            public int number_of_points { get; set; }
                            public List<double> x { get; set; }
                            public List<double> y { get; set; }
                            public Dictionary<string, POINT> point { get; set; }
                            public string input_file_name { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }

                            public class POINT
                            {
                                public double x { get; set; }
                                public double y { get; set; }
                            }
                        }
                    }
                    public class PIERS
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public int format { get; set; }
                        public int number_of_piers { get; set; }
                        public Dictionary<string, PIER> pier { get; set; }

                        public class PIER
                        {
                            public string Name { get; set; }
                            public int include { get; set; }
                            public int input_format { get; set; }
                            public string coordinate_type { get; set; }
                            public double x { get; set; }
                            public double y { get; set; }
                            public float theta { get; set; }
                            public float lamda { get; set; }
                            public int number_of_sections { get; set; }
                            public List<TYPE> type { get; set; }
                            public List<float> height { get; set; }
                            public List<float> length { get; set; }
                            public List<float> width { get; set; }
                            public List<float> radious { get; set; }

                            public enum TYPE
                            {
                                Circular = 0,
                                Rectangle = 1,
                                Elliptical = 2
                            }
                        }
                    }
                    public class TURBINES
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public int format { get; set; }
                        public int number_of_turbines { get; set; }
                        public int output_type { get; set; }
                        public int output_frequency { get; set; }
                        public string output_file_name { get; set; }
                        public Dictionary<string, TURBINE> turbine { get; set; }

                        public class TURBINE
                        {
                            public string Name { get; set; }
                            public int include { get; set; }
                            public string coordinate_type { get; set; }
                            public double x { get; set; }
                            public double y { get; set; }
                            public float diameter { get; set; }
                            public float centroid { get; set; }
                            public float drag_coefficient { get; set; }
                            public CORRECTION_FACTOR correction_factor { get; set; }

                            public class CORRECTION_FACTOR
                            {
                                public int Touched { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                            }
                        }
                    }
                }
                public class INITIAL_CONDITIONS
                {
                    public int Touched { get; set; }
                    public int type { get; set; }
                    public float surface_elevation_constant { get; set; }
                    public float u_velocity_constant { get; set; }
                    public float v_velocity_constant { get; set; }
                    public float w_velocity_constant { get; set; }
                }
                public class BOUNDARY_CONDITIONS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public int internal_land_boundary_Type { get; set; }
                    public Dictionary<string, CODE> code { get; set; }

                    public class CODE
                    {
                        public string identifier { get; set; }
                        public TYPE type { get; set; }
                        public int type_interpolation_constrain { get; set; }
                        public int type_secondary { get; set; }
                        public int type_of_vertical_profile { get; set; }
                        public FORMAT format { get; set; }
                        public float constant_value { get; set; }
                        public List<float> constant_values { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public List<int> item_numbers { get; set; }
                        public string item_name { get; set; }
                        public List<string> item_names { get; set; }
                        public TYPE_OF_SOFT_START type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public float reference_value { get; set; }
                        public List<float> reference_values { get; set; }
                        public TYPE_OF_TIME_INTERPOLATION type_of_time_interpolation { get; set; }
                        public int type_of_space_interpolation { get; set; }
                        public List<DATA> data { get; set; }
                        public TYPE_OF_CORIOLIS_CORRECTION type_of_coriolis_correction { get; set; }
                        public TYPE_OF_WIND_CORRECTION type_of_wind_correction { get; set; }
                        public int type_of_tilting { get; set; }
                        public int type_of_tilting_point { get; set; }
                        public int point_tilting { get; set; }
                        public int type_of_radiation_stress_correction { get; set; }
                        public int type_of_pressure_correction { get; set; }
                        public int type_of_radiation_stress_correction2 { get; set; }

                        public enum TYPE
                        {
                            LandZeroNormalVelocity = 1,
                            LandZeroVelocity = 2,
                            SpecifiedVelocity = 4,
                            SpecifiedFluxes = 5,
                            SpecifiedLevel = 6,
                            SpecifiedDischarge = 7,
                            FlatherCondition = 12
                        }
                        public enum FORMAT
                        {
                            Constant = 0,
                            VaryingInTimeConstantAlongBoundary = 1,
                            VaryingInTimeAndAlongBoundary = 3
                        }
                        public enum TYPE_OF_SOFT_START
                        {
                            LinearVariation = 1,
                            SinusVariation = 2
                        }
                        public enum TYPE_OF_TIME_INTERPOLATION
                        {
                            Linear = 1,
                            PiecewiseCubic = 2
                        }
                        public class DATA
                        {
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                            public int type_of_space_interpolation { get; set; }
                        }
                        public enum TYPE_OF_CORIOLIS_CORRECTION
                        {
                            CoriolisCorrectionNotIncluded = 0,
                            CoriolisCorrectionIncluded = 1
                        }
                        public enum TYPE_OF_WIND_CORRECTION
                        {
                            WindCorrectionNotIncluded = 0,
                            WindCorrectionIncluded = 1
                        }
                    }
                }
                public class TEMPERATURE_SALINITY_MODULE
                {
                    public int temperature_mode { get; set; }
                    public int salinity_mode { get; set; }
                    public TIME time { get; set; }
                    public SPACE space { get; set; }
                    public EQUATION equation_TSM { get; set; }
                    public SOLUTION_TECHNIQUE solution_technique { get; set; }
                    public DIFFUSION diffusion { get; set; }
                    public HEAT_EXCHANGE heat_exchange { get; set; }
                    public PRECIPITATION_EVAPORATION precipitation_evaporation { get; set; }
                    public SOURCES sources { get; set; }
                    public INITIAL_CONDITIONS initial_conditions { get; set; }
                    public BOUNDARY_CONDITIONS boundary_conditions { get; set; }

                    public class TIME
                    {
                        public int start_time_step { get; set; }
                        public int time_step_factor { get; set; }
                    }
                    public class SPACE
                    {
                        public int number_of_2D_mesh_geometry { get; set; }
                        public int number_of_3D_mesh_geometry { get; set; }
                    }
                    public class EQUATION
                    {
                        public int Touched { get; set; }
                        public float minimum_temperature { get; set; }
                        public float maximum_temperature { get; set; }
                        public float minimum_salinity { get; set; }
                        public float maximum_salinity { get; set; }
                    }
                    public class SOLUTION_TECHNIQUE
                    {
                        public int Touched { get; set; }
                        public SCHEME_OF_TIME_INTEGRATION scheme_of_time_integration { get; set; }
                        public int scheme_of_space_discretization_horizontal { get; set; }
                        public int scheme_of_space_discretization_vertical { get; set; }
                        public int method_of_space_discretization_horizontal { get; set; }

                        public enum SCHEME_OF_TIME_INTEGRATION
                        {
                            LowOrderFastAlgorithm = 1,
                            HigherOrder = 2
                        }

                    }
                    public class DIFFUSION
                    {
                        public HORIZONTAL_DIFFUSION horizontal_diffusion { get; set; }
                        public VERTICAL_DIFFUSION vertical_diffusion { get; set; }

                        public class HORIZONTAL_DIFFUSION
                        {
                            public int Touched { get; set; }
                            public TYPE type { get; set; }
                            public SCALED_EDDY_VISCOSITY scaled_eddy_viscosity { get; set; }
                            public DIFFUSION_COEFFICIENT diffusion_coefficient { get; set; }

                            public enum TYPE
                            {
                                NoDispersion = 0,
                                ScaledEddyViscosityFormulation = 1,
                                DispersionViscosityFormulation = 2
                            }
                            public class SCALED_EDDY_VISCOSITY
                            {
                                public int format { get; set; }
                                public float sigma { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                            public class DIFFUSION_COEFFICIENT
                            {
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                        }
                        public class VERTICAL_DIFFUSION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public SCALED_EDDY_VISCOSITY scaled_eddy_viscosity { get; set; }
                            public DIFFUSION_COEFFICIENT diffusion_coefficient { get; set; }

                            public class SCALED_EDDY_VISCOSITY
                            {
                                public int format { get; set; }
                                public float sigma { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                            public class DIFFUSION_COEFFICIENT
                            {
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                        }
                    }
                    public class HEAT_EXCHANGE
                    {
                        public int Touched { get; set; }
                        public TYPE type { get; set; }
                        public float Angstroms_law_A { get; set; }
                        public float Angstroms_law_B { get; set; }
                        public float Beers_law_beta { get; set; }
                        public float light_extinction { get; set; }
                        public float displacement_hours { get; set; }
                        public float standard_meridian { get; set; }
                        public float Daltons_law_A { get; set; }
                        public float Daltons_law_B { get; set; }
                        public float sensible_heat_transfer_coefficient_heating { get; set; }
                        public float sensible_heat_transfer_coefficient_cooling { get; set; }
                        public AIR_TEMPERATURE air_temperature { get; set; }
                        public RELATIVE_HUMIDITY relative_humidity { get; set; }
                        public CLEARNESS_COEFFICIENT clearness_coefficient { get; set; }

                        public enum TYPE
                        {
                            HeatExchangeNotIncluded = 0,
                            HeatExchangeIncluded = 1
                        }
                        public class AIR_TEMPERATURE
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                        public class RELATIVE_HUMIDITY
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                        public class CLEARNESS_COEFFICIENT
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                    public class PRECIPITATION_EVAPORATION
                    {
                        public int Touched { get; set; }
                        public TYPE_OF_PRECIPITATION type_of_precipitation { get; set; }
                        public int type_of_evaporation { get; set; }
                        public PRECIPITATION precipitation { get; set; }
                        public EVAPORATION evaporation { get; set; }

                        public enum TYPE_OF_PRECIPITATION
                        {
                            AmbientWaterTemperature = 1,
                            SpecifiedTemperature = 2
                        }
                        public class PRECIPITATION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                        public class EVAPORATION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                    public class SOURCES
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, SOURCE> source { get; set; }
                        public class SOURCE
                        {
                            public string name { get; set; }
                            public int type_of_temperature { get; set; }
                            public int type_of_salinity { get; set; }
                            public TEMPERATURE temperature { get; set; }
                            public SALINITY salinity { get; set; }

                            public class TEMPERATURE
                            {
                                public int Touched { get; set; }
                                public int type { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                            }
                            public class SALINITY
                            {
                                public int Touched { get; set; }
                                public int type { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                            }
                        }
                    }
                    public class INITIAL_CONDITIONS
                    {
                        public int Touched { get; set; }
                        public TEMPERATURE_TSM temperature { get; set; }
                        public SALINITY_TSM salinity { get; set; }

                        public class TEMPERATURE_TSM
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                        public class SALINITY_TSM
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                    public class BOUNDARY_CONDITIONS
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, CODE> code { get; set; }

                        public class CODE
                        {
                            public TEMPERATURE temperature { get; set; }
                            public SALINITY salinity { get; set; }

                            public class TEMPERATURE
                            {
                                public string identifier { get; set; }
                                public int type { get; set; }
                                public int type_interpolation_constrain { get; set; }
                                public int type_secondary { get; set; }
                                public int type_of_vertical_profile { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                                public int type_of_space_interpolation { get; set; }
                                public int type_of_coriolis_correction { get; set; }
                                public int type_of_wind_correction { get; set; }
                                public int type_of_tilting { get; set; }
                                public int type_of_tilting_point { get; set; }
                                public int point_tilting { get; set; }
                                public int type_of_radiation_stress_correction { get; set; }
                                public int type_of_pressure_correction { get; set; }
                                public int type_of_radiation_stress_correction2 { get; set; }
                            }
                            public class SALINITY
                            {
                                public string identifier { get; set; }
                                public int type { get; set; }
                                public int type_interpolation_constrain { get; set; }
                                public int type_secondary { get; set; }
                                public int type_of_vertical_profile { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                                public int type_of_space_interpolation { get; set; }
                                public int type_of_coriolis_correction { get; set; }
                                public int type_of_wind_correction { get; set; }
                                public int type_of_tilting { get; set; }
                                public int type_of_tilting_point { get; set; }
                                public int point_tilting { get; set; }
                                public int type_of_radiation_stress_correction { get; set; }
                                public int type_of_pressure_correction { get; set; }
                                public int type_of_radiation_stress_correction2 { get; set; }
                            }
                        }
                    }
                }
                public class TURBULENCE_MODULE
                {
                    public int mode { get; set; }
                    public TIME time { get; set; }
                    public SPACE space { get; set; }
                    public EQUATION equation { get; set; }
                    public SOLUTION_TECHNIQUE solution_technique { get; set; }
                    public DIFFUSION diffusion { get; set; }
                    public SOURCES sources { get; set; }
                    public INITIAL_CONDITIONS initial_conditions { get; set; }
                    public BOUNDARY_CONDITIONS boundary_conditions { get; set; }

                    public class TIME
                    {
                        public int start_time_step { get; set; }
                        public int time_step_factor { get; set; }
                    }
                    public class SPACE
                    {
                        public int number_of_2D_mesh_geometry { get; set; }
                        public int number_of_3D_mesh_geometry { get; set; }
                    }
                    public class EQUATION
                    {
                        public int Touched { get; set; }
                        public float c1e { get; set; }
                        public float c2e { get; set; }
                        public float c3e { get; set; }
                        public float prandtl_number { get; set; }
                        public float cmy { get; set; }
                        public double minimum_kinetic_energy { get; set; }
                        public double maximum_kinetic_energy { get; set; }
                        public double minimum_dissipation_of_kinetic_energy { get; set; }
                        public double maximum_dissipation_of_kinetic_energy { get; set; }
                        public double surface_dissipation_parameter { get; set; }
                        public float Ri_damping { get; set; }
                    }
                    public class SOLUTION_TECHNIQUE
                    {
                        public int Touched { get; set; }
                        public int scheme_of_time_integration { get; set; }
                        public int scheme_of_space_discretization_horizontal { get; set; }
                        public int scheme_of_space_discretization_vertical { get; set; }
                        public int method_of_space_discretization_horizontal { get; set; }
                    }
                    public class DIFFUSION
                    {
                        public int Touched { get; set; }
                        public float sigma_k_h { get; set; }
                        public float sigma_e_h { get; set; }
                        public float sigma_k { get; set; }
                        public float sigma_e { get; set; }
                    }
                    public class SOURCES
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, SOURCE> source { get; set; }

                        public class SOURCE
                        {
                            public KINETIC_ENERGY kinetic_energy { get; set; }
                            public DISSIPATION_OF_KINETIC_ENERGY dissipation_of_kinetic_energy { get; set; }

                            public class KINETIC_ENERGY
                            {
                                public int Touched { get; set; }
                                public int type { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                            }
                            public class DISSIPATION_OF_KINETIC_ENERGY
                            {
                                public int Touched { get; set; }
                                public int type { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                            }
                        }
                    }
                    public class INITIAL_CONDITIONS
                    {
                        public int Touched { get; set; }
                        public KINETIC_ENERGY kinetic_energy { get; set; }
                        public DISSIPATION_OF_KINETIC_ENERGY dissipation_of_kinetic_energy { get; set; }

                        public class KINETIC_ENERGY
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }

                        public class DISSIPATION_OF_KINETIC_ENERGY
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                    public class BOUNDARY_CONDITIONS
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, CODE> code { get; set; }

                        public class CODE
                        {
                            public KINETIC_ENERGY kinetic_energy { get; set; }
                            public DISSIPATION_OF_KINETIC_ENERGY dissipation_of_kinetic_energy { get; set; }

                            public class KINETIC_ENERGY
                            {
                                public string identifier { get; set; }
                                public int type { get; set; }
                                public int type_interpolation_constrain { get; set; }
                                public int type_secondary { get; set; }
                                public int type_of_vertical_profile { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                                public int type_of_space_interpolation { get; set; }
                                public int type_of_coriolis_correction { get; set; }
                                public int type_of_wind_correction { get; set; }
                                public int type_of_tilting { get; set; }
                                public int type_of_tilting_point { get; set; }
                                public int point_tilting { get; set; }
                                public int type_of_radiation_stress_correction { get; set; }
                                public int type_of_pressure_correction { get; set; }
                                public int type_of_radiation_stress_correction2 { get; set; }
                            }
                            public class DISSIPATION_OF_KINETIC_ENERGY
                            {
                                public string identifier { get; set; }
                                public int type { get; set; }
                                public int type_interpolation_constrain { get; set; }
                                public int type_secondary { get; set; }
                                public int type_of_vertical_profile { get; set; }
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                                public int type_of_soft_start { get; set; }
                                public int soft_time_interval { get; set; }
                                public int reference_value { get; set; }
                                public int type_of_time_interpolation { get; set; }
                                public int type_of_space_interpolation { get; set; }
                                public int type_of_coriolis_correction { get; set; }
                                public int type_of_wind_correction { get; set; }
                                public int type_of_tilting { get; set; }
                                public int type_of_tilting_point { get; set; }
                                public int point_tilting { get; set; }
                                public int type_of_radiation_stress_correction { get; set; }
                                public int type_of_pressure_correction { get; set; }
                                public int type_of_radiation_stress_correction2 { get; set; }
                            }
                        }
                    }
                }
                public class DECOUPLING
                {
                    public int Touched { get; set; }
                    public int type { get; set; }
                    public string file_name_flux { get; set; }
                    public string file_name_area { get; set; }
                    public string file_name_volume { get; set; }
                    public string specification_file { get; set; }
                    public int first_time_step { get; set; }
                    public int last_time_step { get; set; }
                    public int time_step_frequency { get; set; }
                }
                public class OUTPUTS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public int number_of_outputs { get; set; }
                    public Dictionary<string, OUTPUT> output { get; set; }

                    public class OUTPUT
                    {
                        public int Touched { get; set; }
                        public int include { get; set; }
                        public string title { get; set; }
                        public string file_name { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public int flood_and_dry { get; set; }
                        public string coordinate_type { get; set; }
                        public int zone { get; set; }
                        public string input_file_name { get; set; }
                        public int input_format { get; set; }
                        public int interpolation_type { get; set; }
                        public int first_time_step { get; set; }
                        public int last_time_step { get; set; }
                        public int time_step_frequency { get; set; }
                        public int number_of_points { get; set; }
                        public Dictionary<string, POINT> point { get; set; }
                        public LINE line { get; set; }
                        public AREA area { get; set; }
                        public PARAMETERS_2D parameters_2d { get; set; }
                        public PARAMETERS_3D parameters_3d { get; set; }
                        public DISCHARGE discharge { get; set; }
                        public MASSBUDGET massbudget { get; set; }

                        public class POINT
                        {
                            public string name { get; set; }
                            public double x { get; set; }
                            public double y { get; set; }
                            public double z { get; set; }
                            public int layer { get; set; }
                        }
                        public class LINE
                        {
                            public int npoints { get; set; }
                            public double x_first { get; set; }
                            public double y_first { get; set; }
                            public double z_first { get; set; }
                            public double x_last { get; set; }
                            public double y_last { get; set; }
                            public double z_last { get; set; }
                        }
                        public class AREA
                        {
                            public int number_of_points { get; set; }
                            public Dictionary<string, POINT> point { get; set; }
                            public int layer_min { get; set; }
                            public int layer_max { get; set; }

                            public class POINT
                            {
                                public double x { get; set; }
                                public double y { get; set; }
                            }
                        }
                        public class PARAMETERS_2D
                        {
                            public int Touched { get; set; }
                            public int SURFACE_ELEVATION { get; set; }
                            public int STILL_WATER_DEPTH { get; set; }
                            public int TOTAL_WATER_DEPTH { get; set; }
                            public int U_VELOCITY { get; set; }
                            public int V_VELOCITY { get; set; }
                            public int P_FLUX { get; set; }
                            public int Q_FLUX { get; set; }
                            public int DENSITY { get; set; }
                            public int TEMPERATURE { get; set; }
                            public int SALINITY { get; set; }
                            public int CURRENT_SPEED { get; set; }
                            public int CURRENT_DIRECTION { get; set; }
                            public int WIND_U_VELOCITY { get; set; }
                            public int WIND_V_VELOCITY { get; set; }
                            public int AIR_PRESSURE { get; set; }
                            public int PRECIPITATION { get; set; }
                            public int EVAPORATION { get; set; }
                            public int DRAG_COEFFICIENT { get; set; }
                            public int EDDY_VISCOSITY { get; set; }
                            public int CFL_NUMBER { get; set; }
                            public int CONVERGENCE_ANGLE { get; set; }
                            public int AREA { get; set; }
                        }
                        public class PARAMETERS_3D
                        {
                            public int Touched { get; set; }
                            public int U_VELOCITY { get; set; }
                            public int V_VELOCITY { get; set; }
                            public int W_VELOCITY { get; set; }
                            public int WS_VELOCITY { get; set; }
                            public int DENSITY { get; set; }
                            public int TEMPERATURE { get; set; }
                            public int SALINITY { get; set; }
                            public int TURBULENT_KINETIC_ENERGY { get; set; }
                            public int DISSIPATION_OF_TKE { get; set; }
                            public int CURRENT_SPEED { get; set; }
                            public int CURRENT_DIRECTION_HORIZONTAL { get; set; }
                            public int CURRENT_DIRECTION_VERTICAL { get; set; }
                            public int HORIZONTAL_EDDY_VISCOSITY { get; set; }
                            public int VERTICAL_EDDY_VISCOSITY { get; set; }
                            public int CFL_NUMBER { get; set; }
                            public int VOLUME { get; set; }
                        }
                        public class DISCHARGE
                        {
                            public int Touched { get; set; }
                            public int discharge { get; set; }
                            public int ACCUMULATED_DISCHARGE { get; set; }
                            public int FLOW { get; set; }
                            public int TEMPERATURE { get; set; }
                            public int SALINITY { get; set; }
                        }
                        public class MASSBUDGET
                        {
                            public int Touched { get; set; }
                            public int DISCHARGE { get; set; }
                            public int ACCUMULATED_DISCHARGE { get; set; }
                            public int FLOW { get; set; }
                            public int TEMPERATURE { get; set; }
                            public int SALINITY { get; set; }
                        }
                    }
                }
            }

            public class TRANSPORT_MODULE
            {
                public int mode { get; set; }
                public EQUATION equation { get; set; }
                public TIME time { get; set; }
                public SPACE space_TRM { get; set; }
                public COMPONENTS components { get; set; }
                public SOLUTION_TECHNIQUE solution_technique { get; set; }
                public HYDRODYNAMIC_CONDITIONS hydrodynamic_conditions { get; set; }
                public DISPERSION dispersion { get; set; }
                public DECAY decay { get; set; }
                public PRECIPITATION_EVAPORATION precipitation_evaporation { get; set; }
                public SOURCES sources { get; set; }
                public INITIAL_CONDITIONS initial_conditions { get; set; }
                public BOUNDARY_CONDITIONS boundary_conditions { get; set; }
                public OUTPUTS outputs { get; set; }

                public class EQUATION
                {
                    public int formulation { get; set; }
                }
                public class TIME
                {
                    public int start_time_step { get; set; }
                    public int time_step_factor { get; set; }
                }
                public class SPACE
                {
                    public int number_of_2D_mesh_geometry { get; set; }
                    public int number_of_2D_mesh_velocity { get; set; }
                    public int number_of_2D_mesh_concentration { get; set; }
                    public int number_of_3D_mesh_geometry { get; set; }
                    public int number_of_3D_mesh_velocity { get; set; }
                    public int number_of_3D_mesh_concentration { get; set; }
                }
                public class COMPONENTS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public int number_of_components { get; set; }
                    public Dictionary<string, COMPONENT> component { get; set; }

                    public class COMPONENT
                    {
                        public int Touched { get; set; }
                        public string name { get; set; }
                        public int type { get; set; }
                        public int dimension { get; set; }
                        public string description { get; set; }
                        public int EUM_type { get; set; }
                        public int EUM_unit { get; set; }
                        public string unit { get; set; }
                        public float minimum_value { get; set; }
                        public double maximum_value { get; set; }
                    }
                }
                public class SOLUTION_TECHNIQUE
                {
                    public int Touched { get; set; }
                    public int scheme_of_time_integration { get; set; }
                    public int scheme_of_space_discretization_horizontal { get; set; }
                    public int scheme_of_space_discretization_vertical { get; set; }
                    public int method_of_space_discretization_horizontal { get; set; }
                }
                public class HYDRODYNAMIC_CONDITIONS
                {
                    public int Touched { get; set; }
                    public int type { get; set; }
                    public int format { get; set; }
                    public float surface_elevation_constant { get; set; }
                    public float u_velocity_constant { get; set; }
                    public float v_velocity_constant { get; set; }
                    public float w_velocity_constant { get; set; }
                    public string file_name { get; set; }
                }
                public class DISPERSION
                {
                    public HORIZONTAL_DISPERSION horizontal_dispersion { get; set; }
                    public VERTICAL_DISPERSION vertical_dispersion { get; set; }

                    public class HORIZONTAL_DISPERSION
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, COMPONENT> component { get; set; }

                        public class COMPONENT
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public SCALED_EDDY_VISCOSITY scaled_eddy_viscosity { get; set; }
                            public DISPERSION_COEFFICIENT dispersion_coefficient { get; set; }

                            public class SCALED_EDDY_VISCOSITY
                            {
                                public int format { get; set; }
                                public float sigma { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                            public class DISPERSION_COEFFICIENT
                            {
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                        }
                    }
                    public class VERTICAL_DISPERSION
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, COMPONENT> component { get; set; }

                        public class COMPONENT
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public SCALED_EDDY_VISCOSITY scaled_eddy_viscosity { get; set; }
                            public DISPERSION_COEFFICIENT dispersion_coefficient { get; set; }

                            public class SCALED_EDDY_VISCOSITY
                            {
                                public int format { get; set; }
                                public float sigma { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                            public class DISPERSION_COEFFICIENT
                            {
                                public int format { get; set; }
                                public float constant_value { get; set; }
                                public string file_name { get; set; }
                                public int item_number { get; set; }
                                public string item_name { get; set; }
                            }
                        }
                    }
                }
                public class DECAY
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, COMPONENT> component { get; set; }

                    public class COMPONENT
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public double constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                }
                public class PRECIPITATION_EVAPORATION
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, COMPONENT> component { get; set; }

                    public class COMPONENT
                    {
                        public int Touched { get; set; }
                        public int type_of_precipitation { get; set; }
                        public int type_of_evaporation { get; set; }
                        public PRECIPITATION precipitation { get; set; }
                        public EVAPORATION evaporation { get; set; }

                        public class PRECIPITATION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                        public class EVAPORATION
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                }
                public class SOURCES
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, SOURCE> source { get; set; }

                    public class SOURCE
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public string name { get; set; }
                        public Dictionary<string, COMPONENT> component { get; set; }

                        public class COMPONENT
                        {
                            public int type_of_component { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                        }
                    }
                }
                public class INITIAL_CONDITIONS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, COMPONENT> component { get; set; }

                    public class COMPONENT
                    {
                        public int Touched { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public float constant_value { get; set; }
                        public string file_name { get; set; }
                        public int item_number { get; set; }
                        public string item_name { get; set; }
                        public int type_of_soft_start { get; set; }
                        public int soft_time_interval { get; set; }
                        public int reference_value { get; set; }
                        public int type_of_time_interpolation { get; set; }
                    }
                }
                public class BOUNDARY_CONDITIONS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public Dictionary<string, CODE> code { get; set; }

                    public class CODE
                    {
                        public int Touched { get; set; }
                        public int MzSEPfsListItemCount { get; set; }
                        public Dictionary<string, COMPONENT> component { get; set; }

                        public class COMPONENT
                        {
                            public int Touched { get; set; }
                            public int type { get; set; }
                            public int format { get; set; }
                            public float constant_value { get; set; }
                            public string file_name { get; set; }
                            public int item_number { get; set; }
                            public string item_name { get; set; }
                            public int type_of_soft_start { get; set; }
                            public int soft_time_interval { get; set; }
                            public int reference_value { get; set; }
                            public int type_of_time_interpolation { get; set; }
                            public int type_of_space_interpolation { get; set; }
                        }
                    }
                }
                public class OUTPUTS
                {
                    public int Touched { get; set; }
                    public int MzSEPfsListItemCount { get; set; }
                    public int number_of_outputs { get; set; }
                    public Dictionary<string, OUTPUT> output { get; set; }

                    public class OUTPUT
                    {
                        public int Touched { get; set; }
                        public int include { get; set; }
                        public string title { get; set; }
                        public string file_name { get; set; }
                        public int type { get; set; }
                        public int format { get; set; }
                        public int flood_and_dry { get; set; }
                        public string coordinate_type { get; set; }
                        public int zone { get; set; }
                        public string input_file_name { get; set; }
                        public int input_format { get; set; }
                        public int interpolation_type { get; set; }
                        public int first_time_step { get; set; }
                        public int last_time_step { get; set; }
                        public int time_step_frequency { get; set; }
                        public int number_of_points { get; set; }
                        public Dictionary<string, POINT> point { get; set; }
                        public LINE line { get; set; }
                        public AREA area { get; set; }
                        public PARAMETERS_2D parameters_2d { get; set; }
                        public PARAMETERS_3D parameters_3d { get; set; }
                        public DISCHARGE discharge { get; set; }
                        public MASSBUDGET massbudget { get; set; }

                        public class POINT
                        {
                            public string name { get; set; }
                            public double x { get; set; }
                            public double y { get; set; }
                            public double z { get; set; }
                            public int layer { get; set; }
                        }
                        public class LINE
                        {
                            public int npoints { get; set; }
                            public double x_first { get; set; }
                            public double y_first { get; set; }
                            public double z_first { get; set; }
                            public double x_last { get; set; }
                            public double y_last { get; set; }
                            public double z_last { get; set; }
                        }
                        public class AREA
                        {
                            public int number_of_points { get; set; }
                            public Dictionary<string, POINT> point { get; set; }
                            public int layer_min { get; set; }
                            public int layer_max { get; set; }

                            public class POINT
                            {
                                public double x { get; set; }
                                public double y { get; set; }
                            }
                        }
                        public class PARAMETERS_2D
                        {
                            public int Touched { get; set; }
                            public int COMPONENT_1 { get; set; }
                            public int U_VELOCITY { get; set; }
                            public int V_VELOCITY { get; set; }
                            public int CFL_NUMBER { get; set; }
                        }
                        public class PARAMETERS_3D
                        {
                            public int Touched { get; set; }
                            public int COMPONENT_1 { get; set; }
                            public int U_VELOCITY { get; set; }
                            public int V_VELOCITY { get; set; }
                            public int W_VELOCITY { get; set; }
                            public int CFL_NUMBER { get; set; }
                        }
                        public class DISCHARGE
                        {
                            public int Touched { get; set; }
                            public int COMPONENT_1 { get; set; }
                        }
                        public class MASSBUDGET
                        {
                            public int Touched { get; set; }
                            public int COMPONENT_1 { get; set; }
                        }
                    }
                }
            }
        }
        public class SYSTEM
        {
            public string ResultRootFolder { get; set; }
            public bool UseCustomResultFolder { get; set; }
            public string CustomResultFolder { get; set; }
        }
    }
}
