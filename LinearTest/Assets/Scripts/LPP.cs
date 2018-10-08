using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace LPPNamespace
{
    public class LPP : MonoBehaviour
    {
        public ObjectiveFunction ObjFunc;
        public Constraint[] Constraints;
        public double[] Variables;

        private void Start()
        {
        }

        public void StartLPP(List<double> listOfValues)
        {

            double[] coeff = new double[] { 7, 5, 5, 7, 7, 3, 0.3, 5.4, 3, 0.7, 10, 10, 7.1, 5, 9, 9.5, 8, 7.5, 6, 4, 3, 7, 10, 8, 5, 10, 10, 10, 6, 2, -1, -1, 7.2, 7.2, 5.0, 7.5, 6, -100, -100, -100, -100, -100, -100, -100, 0, -100 };
            double restrain = 100; //x47
            double restrainNeg = -100; //x47

            double Al_Val = listOfValues[0];//793; //x48
            double As_Val = listOfValues[1];//0.07; //x49
            double Ba_Val = listOfValues[2];//2.7; //x50
            double Ca_Val = listOfValues[3];//33; //x51
            double Cu_Val = listOfValues[4];//5.15; //x52
            double Fe_Val = listOfValues[5];//151; //x53
            double K_Val = listOfValues[6];//184; //x54
            double Mg_Val = listOfValues[7];//20; //x55
            double Mn_Val = listOfValues[8];//5.99; //x56 
            double Mo_Val = listOfValues[9];//0.09; //x57
            double Na_Val = listOfValues[10];//439; //x58
            double P_Val = listOfValues[11];//7.2; //x59
            double S_Val = listOfValues[12];//2; //x60
            double Ti_Val = listOfValues[13];//22; //x61
            double W_Val = listOfValues[14];//0.06; //x62
            double Other_Val = listOfValues[15];//8334.74; //x63

            double[] constcoeff = new double[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            double[] constcoeffNeg = new double[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

            double[] Al_const = new double[] { 0, 10.41, 10.4, 9.817163594, 9.85, 15.3732, 0, 0, 0, 0, 9.6754, 10.0939, 19.5, 16.46, 7.5391, 7.3702, 1.5633, 3.42, 0.9295, 12.5294, 17.8137858, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 20.75, 9.44, 0, 13.07, 18.03, 0, 0, 0, 0, 0, 0, 0, 2.05, 0 };
            double[] As_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 46, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] Ba_const = new double[] { 0, 0, 0, 0, 0.25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 58.84, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 };
            double[] Ca_const = new double[] { 0, 0.38, 0, 0.12149786, 0, 7.8908, 40.04426534, 20.0185592, 21.73, 29.44, 0.1066, 0.0377, 0, 0, 0.0156, 0.1426, 8.7168, 8.63, 9.3243, 16.5257, 17.63863053, 0, 39.05798859, 0, 0, 0, 0, 0, 0, 20.44022825, 0, 0, 0.04, 0.49, 0, 1.43, 0.45, 0, 0, 100, 0, 0, 0, 0, 0, 0 };
            double[] Cu_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 34.5, 0, 0, 0, 0, 79.85, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 33.86, 0 };
            double[] Fe_const = new double[] { 0, 0.05, 0.04, 0.0979097, 0.08, 0.199, 0, 12.55342226, 0, 0, 20.3305, 10.5241, 0.08, 3.19, 7.9141, 13.0428, 12.7119, 11.26, 6.3783, 9.494, 0, 72.34827478, 0, 30.49887156, 46.59103012, 0, 34.3, 0, 0, 0, 0, 0, 0.16, 2.23, 33.45, 9.21, 2.38, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] K_const = new double[] { 0, 0.11, 0.24, 13.33707643, 13.5, 0.1423, 0, 0, 0, 0, 0.0161, 0.0335, 8.37, 9.14, 7.8631, 7.3473, 0.1038, 0.43, 0.0668, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.17, 0.37, 7.81, 0.04, 0.01, 0, 0, 0, 0, 100, 0, 0, 0, 0 };
            double[] Mg_const = new double[] { 0, 0.01, 0.02, 0, 0, -0.0012, 0, 6.071985112, 13.18, 0, 9.3554, 14.3654, 0.05, 0.81, 11.8337, 8.2094, 8.065, 8.03, 11.4574, 0.0194, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.04, 1.87, 0, 5.23, 5.69, 0, 0, 0, 0, 0, 100, 0, 0, 0 };
            double[] Mn_const = new double[] { 0, 0, 0, 0, 0.03, -0.0065, 0, 0, 0, 0, 0.0787, 0.0387, 0.093, 0, 0.052, 0.0686, 0.0905, 0.48, 0.1231, 0.1798, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.023, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100 };
            double[] Mo_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 59.9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] Na_const = new double[] { 0, 7.8, 8.21, 0.126114876, 0.31, 3.8507, 0, 0, 0, 0, 0.0455, 0.0293, 0.47, 0.08, 0.0718, 0.0769, 0.204, 0.78, 0.2079, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.07, 1.61, 0, 1.26, 1.46, 0, 0, 0, 0, 0, 0, 100, 0, 0 };
            double[] P_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18.17529238, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] S_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 23.55, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 35, 53.4, 0, 19.7, 40.1, 20.15, 0, 0, 13.74, 0, 0, 12.81, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] Ti_const = new double[] { 0, 0, 0, 0, 0.03, 0.002, 0, 0, 0, 0, 0.0163, 0.0267, 0, 0.1, 1.0092, 0.9201, 0.0574, 0.53, 0.0767, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24.46569855, 59.96494742, 0, 0.05, 0.04, 0, 1.51, 0.24, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] W_const = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88.15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] Other_const = new double[] { 100.00, 81.24, 81.09, 76.50, 75.95, 72.55, 59.96, 61.36, 65.09, 47.01, 60.38, 64.85, 71.44, 70.22, 63.70, 62.82, 68.49, 66.44, 71.44, 61.25, 64.55, 27.65, 42.77, 0.00, 0.01, 11.85, 0.00, 0.00, 0.00, 55.09, 40.04, 27.42, 78.72, 83.93, 45.93, 68.25, 71.74, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 0.00, 64.09, 0.00 };

            double[] Al_constNeg = new double[] { 0, -10.41, -10.4, -9.817163594, -9.85, -15.3732, -0, -0, -0, -0, -9.6754, -10.0939, -19.5, -16.46, -7.5391, -7.3702, -1.5633, -3.42, -0.9295, -12.5294, -17.8137858, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -20.75, -9.44, -0, -13.07, -18.03, -0, -0, -0, -0, -0, -0, -0, -2.05, -0 };
            double[] As_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -46, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -100, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] Ba_constNeg = new double[] { 0, -0, -0, -0, -0.25, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -58.84, -0, -0, -0, -0, -0, -0, -100, -0, -0, -0, -0, -0, -0, -0 };
            double[] Ca_constNeg = new double[] { 0, -0.38, -0, -0.12149786, -0, -7.8908, -40.04426534, -20.0185592, -21.73, -29.44, -0.1066, -0.0377, -0, -0, -0.0156, -0.1426, -8.7168, -8.63, -9.3243, -16.5257, -17.63863053, -0, -39.05798859, -0, -0, -0, -0, -0, -0, -20.44022825, -0, -0, -0.04, -0.49, -0, -1.43, -0.45, -0, -0, -100, -0, -0, -0, -0, -0, -0 };
            double[] Cu_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -34.5, -0, -0, -0, -0, -79.85, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -100, -0, -0, -0, -33.86, -0 };
            double[] Fe_constNeg = new double[] { 0, -0.05, -0.04, -0.0979097, -0.08, -0.199, -0, -12.55342226, -0, -0, -20.3305, -10.5241, -0.08, -3.19, -7.9141, -13.0428, -12.7119, -11.26, -6.3783, -9.494, -0, -72.34827478, -0, -30.49887156, -46.59103012, -0, -34.3, -0, -0, -0, -0, -0, -0.16, -2.23, -33.45, -9.21, -2.38, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] K_constNeg = new double[] { 0, -0.11, -0.24, -13.33707643, -13.5, -0.1423, -0, -0, -0, -0, -0.0161, -0.0335, -8.37, -9.14, -7.8631, -7.3473, -0.1038, -0.43, -0.0668, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0.17, -0.37, -7.81, -0.04, -0.01, -0, -0, -0, -0, -100, -0, -0, -0, -0 };
            double[] Mg_constNeg = new double[] { 0, -0.01, -0.02, -0, -0, 0.0012, -0, -6.071985112, -13.18, -0, -9.3554, -14.3654, -0.05, -0.81, -11.8337, -8.2094, -8.065, -8.03, -11.4574, -0.0194, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0.04, -1.87, -0, -5.23, -5.69, -0, -0, -0, -0, -0, -100, -0, -0, -0 };
            double[] Mn_constNeg = new double[] { 0, -0, -0, -0, -0.03, 0.0065, -0, -0, -0, -0, -0.0787, -0.0387, -0.093, -0, -0.052, -0.0686, -0.0905, -0.48, -0.1231, -0.1798, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0.023, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -100 };
            double[] Mo_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -59.9, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] Na_constNeg = new double[] { 0, -7.8, -8.21, -0.126114876, -0.31, -3.8507, -0, -0, -0, -0, -0.0455, -0.0293, -0.47, -0.08, -0.0718, -0.0769, -0.204, -0.78, -0.2079, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0.07, -1.61, -0, -1.26, -1.46, -0, -0, -0, -0, -0, -0, -100, -0, -0 };
            double[] P_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -18.17529238, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] S_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -23.55, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -35, -53.4, -0, -19.7, -40.1, -20.15, -0, -0, -13.74, -0, -0, -12.81, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] Ti_constNeg = new double[] { 0, -0, -0, -0, -0.03, -0.002, -0, -0, -0, -0, -0.0163, -0.0267, -0, -0.1, -1.0092, -0.9201, -0.0574, -0.53, -0.0767, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -24.46569855, -59.96494742, -0, -0.05, -0.04, -0, -1.51, -0.24, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] W_constNeg = new double[] { 0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -88.15, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0, -0 };
            double[] Other_constNeg = new double[] { -100.00, -81.24, -81.09, -76.50, -75.95, -72.55, -59.96, -61.36, -65.09, -47.01, -60.38, -64.85, -71.44, -70.22, -63.70, -62.82, -68.49, -66.44, -71.44, -61.25, -64.55, -27.65, -42.77, -0.00, -0.01, -11.85, -0.00, -0.00, -0.00, -55.09, -40.04, -27.42, -78.72, -83.93, -45.93, -68.25, -71.74, -0.00, -0.00, -0.00, -0.00, -0.00, -0.00, -0.00, -64.09, -0.00 };

            double Kao_Val = 3;
            double Smec_Val = 2;
            double[] KaoConst = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] SmecConst = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            //double[] constcoeff3 = new double[] { 0, 0, 0, 1 };
            //double[] constcoeff4 = new double[] { 1, 1, 1, 1 };
            ObjectiveFunction objf = new ObjectiveFunction(coeff);
            //double[] variables = new double[] { 1, 2, 3, 4, 5 };
            //double newDoub = objf.Value(variables);

            Constraint[] cnts = new Constraint[] { new Constraint(constcoeff, restrain),//                new Constraint(constcoeffNeg, restrainNeg),
                new Constraint(Al_const, Al_Val),
                new Constraint(As_const, As_Val),
                new Constraint(Ba_const, Ba_Val),
                new Constraint(Ca_const, Ca_Val),
                new Constraint(Cu_const, Cu_Val),
                new Constraint(Fe_const, Fe_Val),
                new Constraint(K_const, K_Val),
                new Constraint(Mg_const, Mg_Val),
                new Constraint(Mn_const, Mn_Val),
                new Constraint(Mo_const, Mo_Val),
                new Constraint(Na_const, Na_Val),
                new Constraint(P_const, P_Val),
                new Constraint(S_const, S_Val),
                new Constraint(Ti_const, Ti_Val),
                new Constraint(W_const, W_Val),
                new Constraint(Other_const, Other_Val),// };/*
                new Constraint(Al_constNeg, -Al_Val),
                new Constraint(As_constNeg, -As_Val),
                new Constraint(Ba_constNeg, -Ba_Val),
                new Constraint(Ca_constNeg, -Ca_Val),
                new Constraint(Cu_constNeg, -Cu_Val),
                new Constraint(Fe_constNeg, -Fe_Val),
                new Constraint(K_constNeg, -K_Val),
                new Constraint(Mg_constNeg, -Mg_Val),
                new Constraint(Mn_constNeg, -Mn_Val),
                new Constraint(Mo_constNeg, -Mo_Val),
                new Constraint(Na_constNeg, -Na_Val),
                new Constraint(P_constNeg, -P_Val),
                new Constraint(S_constNeg, -S_Val),
                new Constraint(Ti_constNeg, -Ti_Val),
                new Constraint(W_constNeg, -W_Val),
                new Constraint(Other_constNeg, -Other_Val),
                new Constraint(KaoConst, Kao_Val),
                new Constraint(SmecConst, Smec_Val)
            };//, new Constraint(constcoeff3, restrain3), new Constraint(constcoeff4, restrain4) };*/

            /*double[] coeff = new double[] { 2, 3, 4, 5 };
            double restrain = 100;
            double[] constcoeff = new double[] { 1, 1, 1, 1 };
            double[] constcoeff2 = new double[] { 6, 7, 8, 9 };
            double[] constcoeff2Neg = new double[] { -6, -7, -8, -9 };
            double restrain2 = 50;

            ObjectiveFunction objf = new ObjectiveFunction(coeff);
            Constraint[] cnts = new Constraint[] { new Constraint(constcoeff, restrain), new Constraint(constcoeff2, restrain2), new Constraint(constcoeff2Neg, -restrain2) };*/
            LPP lpp = new LPP(objf, cnts);
            lpp.Solve();
        }

        public LPP(ObjectiveFunction objFunc, Constraint[] constraints)
        {
            this.ObjFunc = objFunc;
            this.Constraints = constraints;
            this.Variables = new double[ObjFunc.VariablesNumber];
        }

        public bool SolutionFound(Dictionary d)
        {
            return d.EntersBasis() == -1;
        }

        public void Solve()
        {
            //Debug.Log("Solve()");
            Dictionary dict = new Dictionary(this);

            if (!dict.IsFeasible()) dict = this.initialize();

            //Debug.Log("Finding solution...");
            //Debug.Log("-------------------------------");
            //dict.DebugOutputGrid(dict.c);
            while (!SolutionFound(dict))
            {
                dict.Print(true);
                dict.Improve();
            }
            dict.Print(true);
            //dict.DebugOutputGrid(dict.c);

            for (int i = 0; i < dict.basic.Length; i++)
                if (dict.basic[i] < Variables.Length + 1)
                    Variables[dict.basic[i] - 1] = 0;
            for (int i = 0; i < dict.slack.Length; i++)
                if (dict.slack[i] < Variables.Length + 1)
                    Variables[dict.slack[i] - 1] = dict.c[i, 0];
            //Debug.Log("done?");
        }

        private Dictionary initialize()
        {
            //Debug.Log("Initialization phase...");
            //Debug.Log("-------------------------------");

            double[] auxC = new double[ObjFunc.VariablesNumber + 1];
            auxC[0] = -1;
            for (int i = 0; i < auxC.Length - 1; i++)
                auxC[i + 1] = 0;
            ObjectiveFunction auxOF = new ObjectiveFunction(auxC);

            Constraint[] auxCS = new Constraint[this.Constraints.Length];
            int leavesBasis = 0;
            double minB = Constraints[0].Restriction;
            for (int i = 0; i < auxCS.Length; i++)
            {
                double[] auxCC = new double[ObjFunc.VariablesNumber + 1];
                auxCC[0] = -1;
                for (int j = 0; j < auxCC.Length - 1; j++)
                    auxCC[j + 1] = Constraints[i].Coefficients[j];
                auxCS[i] = new Constraint(auxCC, Constraints[i].Restriction);
                if (Constraints[i].Restriction < minB)
                {
                    minB = Constraints[i].Restriction; leavesBasis = i;
                }
            }

            LPP auxLPP = new LPP(auxOF, auxCS);
            Dictionary auxD = new Dictionary(auxLPP);
            auxD.Print(true);
            auxD.Recalculate(0, leavesBasis);
            while (!SolutionFound(auxD))
            {
                auxD.Print(preferToLeave: 1);
                auxD.Improve(preferToLeave: 1);
            }
            auxD.Print(preferToLeave: 1);

            int len = auxD.basic.Length;
            int[] bb = new int[len];
            double[,] cc = new double[auxLPP.Constraints.Length, len + 1];
            int[] ss = new int[auxLPP.Constraints.Length];
            for (int i = 0; i < len; i++)
            {
                bb[i] = auxD.basic[i];
                for (int j = 0; j < auxLPP.Constraints.Length; j++)
                    cc[j, i + 1] = auxD.c[j, i + 1];
            }
            for (int j = 0; j < auxLPP.Constraints.Length; j++)
            {
                cc[j, 0] = auxD.c[j, 0];
                ss[j] = auxD.slack[j];
            }

            auxD.a = new double[len - 1];
            auxD.basic = new int[len - 1];
            auxD.slack = new int[auxLPP.Constraints.Length];
            auxD.c = new double[auxLPP.Constraints.Length, len];
            //Debug.Log("auxD.c length = " + auxD.c.GetUpperBound(0));
            //Debug.Log("auxD.c length = " + auxD.c.GetUpperBound(1));
            //Debug.Log("cc length = " + cc.GetUpperBound(0));
            //Debug.Log("cc length = " + cc.GetUpperBound(1));
            //Debug.Log("bb = " + String.Join(", ", new List<int>(bb).ConvertAll(i => i.ToString()).ToArray()));
            for (int i = 0; i < auxLPP.Constraints.Length; i++)
            {
                auxD.c[i, 0] = cc[i, 0];
                int j = 1;
                //auxD.DebugOutputGrid(cc);
                //Debug.Log("<color=green>~~~~~~</color>");
                //auxD.DebugOutputGrid(auxD.c);

                //Debug.Log("<color=yellow>bb.length</color> = " + bb.Length);
                while (bb[j - 1] != 1)
                {
                    //Debug.Log("bb: " + bb[j - 1]);
                    //Debug.Log("auxD.c[" + i + ", " + j + "] = cc[" + i + ", " + j + "]");
                    //Debug.Log(auxD.c[i, j]);
                    //Debug.Log(cc[i, j]);
                    auxD.c[i, j] = cc[i, j];
                    //auxD.DebugOutputGrid(auxD.c);
                    j++;
                    //if (j == bb.Length)
                    //    break;
                }
                while (j < bb.Length)
                {
                    j++;
                    auxD.c[i, j - 1] = cc[i, j];
                }
                auxD.slack[i] = ss[i] - 1;
            }
            int k = 0;
            while (bb[k] != 1)
            {
                auxD.basic[k] = bb[k] - 1;
                k++;
                //if (k == bb.Length-1)
                //    break;
            }
            k++;
            while (k < bb.Length)
            {
                auxD.basic[k - 1] = bb[k] - 1;
                k++;
            }

            auxD.z0 = 0;
            for (int i = 0; i < this.ObjFunc.Coefficients.Length; i++)
                for (int j = 0; j < auxD.slack.Length; j++)
                    if (auxD.slack[j] == i + 1)
                    {
                        auxD.z0 += ObjFunc.Coefficients[i] * auxD.c[j, 0];
                        for (int m = 0; m < ObjFunc.Coefficients.Length; m++)
                            auxD.a[m] += ObjFunc.Coefficients[i] * auxD.c[j, m + 1];
                    }
            auxD.Print(true);
            

            return auxD;
        }
    }

    public class ObjectiveFunction : MonoBehaviour
    {
        private double[] coefficients;
        public int VariablesNumber
        { get { return this.coefficients.Length; } }

        public double[] Coefficients
        { get { return this.coefficients; } }

        public ObjectiveFunction(double[] coefficients)
        {
            this.coefficients = coefficients;
        }

        public double Value(double[] variables)
        {
            double value = 0;
            if (VariablesNumber != variables.Length)
                //Debug.Log("that's a goof");
                //throw new ArgumentException("The number of variables (" + variables.Length + ") shoud be equal to the number of function coefficients (" + this.VariablesNumber + ").");
                
            //Debug.Log("VariablesNumber: " + VariablesNumber);
            if (this.coefficients.Length > 0)
                for (int i = 0; i < VariablesNumber; i++)
                {
                    value += this.coefficients[i] * variables[i];
                    //Debug.Log("value is now " + value + " (" + this.coefficients[i] + " * " + variables[i] + ")");
                }
            return value;
        }
    }

    public class Constraint : MonoBehaviour
    {
        private double[] coefficients;
        private double restriction;
        private int coefficientsNumber;

        public double[] Coefficients
        { get { return this.coefficients; } }

        public double Restriction
        { get { return this.restriction; } }

        public Constraint(double[] coefficients, double restriction)
        {
            this.coefficientsNumber = coefficients.Length;
            this.coefficients = coefficients;
            this.restriction = restriction;
        }
    }

    public class Dictionary : MonoBehaviour
    {
        public double[,] c;
        public double[] a;
        public double z0;
        /// <summary> /// basic variables /// </summary> 
        public int[] basic;
        /// <summary> /// slack variables /// </summary> 
        public int[] slack;

        public Dictionary(LPP lpp)
        {
            z0 = 0;
            this.a = new double[lpp.ObjFunc.VariablesNumber];
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = lpp.ObjFunc.Coefficients[i];
                //Debug.Log("<color=white>a[" + i + "] = </color>" + lpp.ObjFunc.Coefficients[i]);
            }

            this.basic = new int[lpp.ObjFunc.VariablesNumber];
            for (int i = 0; i < lpp.ObjFunc.VariablesNumber; i++)
                this.basic[i] = i + 1;
            this.slack = new int[lpp.Constraints.Length];
            for (int i = 0; i < lpp.Constraints.Length; i++)
                this.slack[i] = lpp.ObjFunc.VariablesNumber + i + 1;


            this.c = new double[lpp.Constraints.Length, lpp.ObjFunc.VariablesNumber + 1];
            for (int i = 0; i < lpp.Constraints.Length; i++)
            {
                this.c[i, 0] = lpp.Constraints[i].Restriction;
                for (int j = 1; j < lpp.ObjFunc.VariablesNumber + 1; j++)
                {
                    this.c[i, j] = -lpp.Constraints[i].Coefficients[j - 1];
                }
            }
            
        }

        public bool IsFeasible()
        {
            for (int i = 0; i < slack.Length; i++)
            {
                //Debug.Log("<color=yellow>checking c[" + i + ", 0] (" + c[i, 0] + ") < 0?</color>");
                if (c[i, 0] < 0) return false;
            }
            return true;
        }

        public void DebugOutputGrid(double[,] grid)
        {
            string textOutput = "";
            for (int y = 0; y <= grid.GetUpperBound(1); y++)
            {
                textOutput += y;
                textOutput += ": ";
                for (int x = 0; x <= grid.GetUpperBound(0); x++)
                {

                    textOutput += grid[x, y].ToString("F3");
                    textOutput += "|";
                }
                textOutput += "\n";
            }
            //Debug.Log(textOutput);
        }

        /// <summary> /// Determines a variable to enter basis /// </summary> 
        /// <returns>Index of a variable to enter basis. If there's no variable to enter, returns -1 </returns> 
        public int EntersBasis()
        {
            int n = -1;
            double maxA = this.a[0];
            if (maxA > 0) n = 0;

            for (int i = 0; i < a.Length; i++)
                if (a[i] > 0 && a[i] > maxA) { maxA = a[i]; n = i; }

            return n;
        }

        /// <summary> /// Determines a variable to leave basis /// </summary> 
        /// <param name="enterIdx">Index of basic variable to enter basis</param> 
        /// <returns>Index of a variable to leave basis. If there's no variable to leave, returns -1 </returns> 

        private int LeavesBasis(int enterIdx, int preferToLeave = -1)
        {
            if (enterIdx == -1) return -1;

            int idxPTL = -1;
            int n = -1;
            double[] dc = new double[slack.Length];

            for (int i = 0; i < dc.Length; i++)
            {
                if (preferToLeave != -1 && slack[i] == preferToLeave)
                    idxPTL = i;
                if (c[i, 1 + enterIdx] < 0)
                    dc[i] = c[i, 0] / c[i, 1 + enterIdx];
                else
                    dc[i] = double.NegativeInfinity;
            }

            double maxDC = dc[0];
            if (maxDC > double.NegativeInfinity) n = 0;
            for (int i = 0; i < dc.Length; i++)
                if (dc[i] <= 0 && dc[i] >= maxDC)
                {
                    maxDC = dc[i];
                    n = i;
                    if (idxPTL != -1 && dc[idxPTL] == maxDC)
                        n = idxPTL;
                }
            return n;
        }

        public void Recalculate(int enterIdx, int leaveIdx)
        {
            // Recalculating coefficients for equation of entering variable 
            c[leaveIdx, 0] = -c[leaveIdx, 0] / c[leaveIdx, enterIdx + 1];
            c[leaveIdx, enterIdx + 1] = 1 / c[leaveIdx, enterIdx + 1];
            for (int j = 0; j < basic.Length; j++)
                if (j != enterIdx)
                    c[leaveIdx, j + 1] = -c[leaveIdx, j + 1] * c[leaveIdx, enterIdx + 1];

            // Recalculating coefficients for other equations 
            for (int i = 0; i < slack.Length; i++)
                if (i != leaveIdx)
                {
                    double oldC = c[i, enterIdx + 1];
                    c[i, 0] = c[i, 0] + c[i, enterIdx + 1] * c[leaveIdx, 0];
                    c[i, enterIdx + 1] = c[i, enterIdx + 1] * c[leaveIdx, enterIdx + 1];
                    for (int j = 0; j < basic.Length; j++)
                        if (j != enterIdx)
                            c[i, j + 1] = c[i, j + 1] + oldC * c[leaveIdx, j + 1];
                }

            // Recalculating coefficients for objective function 
            z0 = z0 + a[enterIdx] * c[leaveIdx, 0];
            double oldA = a[enterIdx];
            a[enterIdx] = a[enterIdx] * c[leaveIdx, enterIdx + 1];
            for (int j = 0; j < basic.Length; j++)
                if (j != enterIdx)
                    a[j] = a[j] + oldA * c[leaveIdx, j + 1];

            // Swaping names of basic and slack variables 
            int valueToSwap = basic[enterIdx];
            basic[enterIdx] = slack[leaveIdx]; slack[leaveIdx] = valueToSwap;
        }

        public void Improve(int preferToLeave = -1)
        {
            int eb = EntersBasis();
            if (eb != -1)
            {
                int lb = LeavesBasis(eb, preferToLeave);
                if (lb != -1)
                    Recalculate(eb, lb);
            }
        }

        public void Print(bool withAnalysis = true, int preferToLeave = -1)
        {
            bool showWorking = false;
            String fullString = "";

            //Debug.Log("Dictionary for LPP:");

            if (showWorking)
            {
                foreach (double d in this.basic)
                {
                    Debug.Log("<color=blue>basic</color> = " + d);
                }
                foreach (double d in this.slack)
                {
                    Debug.Log("<color=red>slack</color> = " + d);
                }
            }
            //DebugOutputGrid(c);
            if (showWorking)
            {
                for (int i = 0; i < this.slack.Length; i++)
                {
                    fullString = "";
                    fullString += ("x" + slack[i] + " = " + c[i, 0]);
                    for (int j = 0; j < this.a.Length; j++)
                        if (c[i, j + 1] < 0)
                        {
                            fullString += ("- " + -c[i, j + 1] + "*x" + basic[j]);
                        }
                        else
                        {
                            fullString += ("+ " + c[i, j + 1] + "*x" + basic[j]);
                        }
                    Debug.Log(fullString);
                }
            }
            if (showWorking)
            {
                foreach (double d in this.basic)
                {
                    Debug.Log("<color=blue>basic</color> = " + d);
                }
                foreach (double d in this.slack)
                {
                    Debug.Log("<color=red>slack</color> = " + d);
                }
            }
            fullString = "";
            if (showWorking)
            {
                fullString += ("z = " + z0);
                for (int j = 0; j < this.a.Length; j++)
                {
                    if (a[j] < 0)
                    {
                        fullString += ("- " + -a[j] + "*x" + basic[j]);
                    }
                    else
                    {
                        fullString += ("+ " + a[j] + "*x" + basic[j]);
                    }
                }
                Debug.Log("<color=orange>" + fullString + "</color>");
            }

            if (false)//(withAnalysis)
            {
                int eb = EntersBasis();
                if (eb == -1)
                {
                    Debug.Log("No variables to enter basis - solution is found.");
                    Debug.Log("The optimal value of objective function is " +  z0);
                    Debug.Log("The optimal solution is:");
                    for (int i = 0; i < basic.Length; i++) Debug.Log("x" + basic[i] + " = 0");
                    for (int i = 0; i < slack.Length; i++) Debug.Log("x" + slack[i] + " = " + c[i, 0]);

                    //Debug.Log("basic = " + String.Join(", ", new List<int>(basic).ConvertAll(i => i.ToString()).ToArray()));
                    //Debug.Log("slack = " + String.Join(", ", new List<int>(slack).ConvertAll(i => i.ToString()).ToArray()));
                }
                else
                {
                    Debug.Log("Enters basis: x" + basic[eb]);
                    if (LeavesBasis(eb) == -1)
                        Debug.Log("No variables to leave basis.");
                    else
                        Debug.Log("Leaves basis: x" + slack[LeavesBasis(eb, preferToLeave)]);
                }
                
            }
        }
    }
}
		