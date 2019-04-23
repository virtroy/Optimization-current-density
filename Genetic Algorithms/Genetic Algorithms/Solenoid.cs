using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithms
{
    class Solenoid
    {
        Form1 oMainWindow;
        const int iArraySize = 100;
        double[,] dPopulation = new double[iArraySize, iArraySize];
        double[] dA1 = new double[iArraySize];
        double[,] dPopCost = new double[iArraySize, iArraySize];
        double[] dPopHzRequired = new double[iArraySize];
        double[,] dPopHzObtained = new double[iArraySize, iArraySize];
        double[,] dKij = new double[iArraySize, iArraySize];
        double[] dPopCostSum = new double[iArraySize];
        double dMinimalCost = 200;
        double[] dMinimalSubject = new double[iArraySize];
        Random oRnd = new Random();
        Random iRnd = new Random();
        int cykl = 0;
        double alpha = 0.7;

        int iPopSize = iArraySize, iChromosomeLength, iRangeXS, iRangeXE, iCycleNumber, iCoefficientOfMutation, iMutationPower, iTournamentSize = 5;
        double dConstField, dLinearFieldS, dLinearFieldE;
        StreamWriter sw = new StreamWriter("Solenoid_log.txt");
        double dR = 10, dSingleA = 3;
        double iMean;

        public void vMain()
        {
                iChromosomeLength = oMainWindow.iTextBoxToInt(99);
                iRangeXS = oMainWindow.iTextBoxToInt(11);
                iRangeXE = oMainWindow.iTextBoxToInt(12);
                iCycleNumber = oMainWindow.iTextBoxToInt(14);
                iCoefficientOfMutation = oMainWindow.iTextBoxToInt(15);
                iMutationPower = oMainWindow.iTextBoxToInt(10);

                vFillA01();
                vFillKIJMatrix();

                if (oMainWindow.radioButton10.Checked)
                {
                    dConstField = oMainWindow.dTextBoxToDouble(17);
                    for (int i = 0; i < iChromosomeLength; i++)
                    {
                        dPopHzRequired[i] = dConstField;
                    }
                }

                if (oMainWindow.radioButton9.Checked)
                {
                    dLinearFieldS = oMainWindow.dTextBoxToDouble(20);
                    dLinearFieldE = oMainWindow.dTextBoxToDouble(19);
                    for (int i = 0; i < iChromosomeLength; i++)
                    {
                        dPopHzRequired[i] = dLinearFieldS + ((i * (dLinearFieldE - dLinearFieldS))) / (iChromosomeLength - 1);
        
                    }
                }

                vInitialPopulation();
                vCost();
                vPrintToFile(0, 0);
                for (int i = 0; i < iCycleNumber; i++)
                {
                    if (oMainWindow.radioButton8.Checked) vSelectionTournament();
                    if (oMainWindow.radioButton7.Checked) vSelectionElite();
                    if (oMainWindow.radioButton12.Checked) vSelectionRoulette();
                    vPairing();
                    vMutation();
                    vCost();
                    vPrintToFile(1, i);
                    vPrintToFile(2, 2);
                }
                cykl++;
                

            if (oMainWindow.checkBox2.Checked) System.Diagnostics.Process.Start("Solenoid_log.txt");
            sw.Close();
        }

        protected double dSingularCost(double[] dVector)
        {
            double dElement=0;
            double dEqual = 0;
            for (int z = 0; z < iChromosomeLength; z++)
            {
                dElement = 0;
                for (int Ki = 0; Ki < iChromosomeLength; Ki++)
                {
                    dElement += dKij[z, Ki] * dVector[Ki];
                }

                dEqual += (dPopHzRequired[z] - dElement) * (dPopHzRequired[z] - dElement);
            }
            return dEqual;
        }

        protected void vCost()
        {
            for (int i = 0; i < iPopSize; i++)
            {
                for (int z = 0; z < iChromosomeLength; z++)
                {
                    dPopHzObtained[i, z] = 0;
                    dPopCost[i, z] = 0;

                    for (int Ki = 0; Ki < iChromosomeLength; Ki++)
                    {
                        dPopHzObtained[i, z] += dKij[z, Ki] * dPopulation[i, Ki];
                    }
                    dPopCost[i, z] = Math.Abs(dPopHzRequired[z] - dPopHzObtained[i, z]) * Math.Abs(dPopHzRequired[z] - dPopHzObtained[i, z]);
                }
            }
            for (int i = 0; i < iPopSize; i++)
            {
                dPopCostSum[i] = 0;
                for (int z = 0; z < iChromosomeLength; z++)
                {
                    dPopCostSum[i] += dPopCost[i, z];
                }
                if (dMinimalCost > dPopCostSum[i])
                {
                    dMinimalCost = dPopCostSum[i];
                    for (int j = 0; j < iChromosomeLength; j++)
                    {
                        dMinimalSubject[j] = dPopulation[i, j];
                    }
                }
            }
            iMean = 0;
            for (int k = 0; k < iPopSize; k++)
            {
                iMean += dPopCostSum[k];
            }

            iMean = iMean / iPopSize;
            
        }

        protected void vMutation()
        {
            int iHalfPopSize = iMethodHalfPopSize();
            int licznik_mutacji = 0;
            double dNewValue, dRandom;
            int itempcounter;
            for (int i = 0; i < iPopSize; i++)
            {
                for (int j = 0; j < iChromosomeLength; j++)
                {
                    int rand = oRnd.Next(0, 100);
                    if (iCoefficientOfMutation > rand)
                    {
                        itempcounter = 0;
                        do
                        {
                            dRandom = oRnd.Next(-iMutationPower, iMutationPower);
                            dNewValue = (dRandom / 100);
                            dNewValue = (1 + dNewValue) * dPopulation[i, j];
                          
                            itempcounter++;
                            if (itempcounter > 10)
                            {
                                dNewValue = dPopulation[i, j];
                                break;
                            }

                        }
                        while (dNewValue <= iRangeXS || dNewValue>= iRangeXE);
                        dPopulation[i, j] = dNewValue;
                        licznik_mutacji++;
                    }
                }
            }
        }

        protected void vSelectionRoulette()
        {
            Random oRnd = new Random();
            double[,] dTemporaryArray = new double[iArraySize, iArraySize];
            double dSum = 0, dSumTemp = 0, dRandom, dMin = 0, dMax = 0;
            int iHalfPopSize = iMethodHalfPopSize();
            double[] dTempCost = dPopCostSum;

            for (int i = 0; i < iPopSize; i++)
            {
                if (dMin > dTempCost[i])
                {
                    dMin = dTempCost[i];
                }
            }

            for (int i = 0; i < iPopSize; i++)
            {
                dSum += dTempCost[i];

                if (dMax < dTempCost[i])
                {
                    dMax = dTempCost[i];
                }
            }

            for (int i = 0; i < iPopSize; i++)
            {
                dTempCost[i] = dMax - dPopulation[i, 3] + 10;
            }

            for (int z = 0; z < iHalfPopSize; z++)
            {
                dRandom = oRnd.NextDouble() * dSum;
                dSumTemp = 0;
                for (int i = 0; i < iPopSize; i++)
                {
                    dSumTemp += dTempCost[i];

                    if (dSumTemp > dRandom)
                    {
                        for (int y = 0; y < iPopSize; y++)
                        {
                            dTemporaryArray[z, y] = dPopulation[i, y];
                        }
                        i = iPopSize + 1;
                    }
                }
            }
            dPopulation = dTemporaryArray;
        }



        protected void vSelectionTournament()
        {
            Random oRnd = new Random();
            double[,] dMatingPool = new double[iArraySize, iArraySize];
            double[] dCostVector = new double[iPopSize];
            double[] dTemp = new double[iChromosomeLength];
            int iRandom;
            int iHalfPopSize = iMethodHalfPopSize();
            int[] Index = new int[iTournamentSize];
 
            for (int i = 0; i < iPopSize; i++)
            {
                for (int j = 0; j < iChromosomeLength; j++)
                {
                    dTemp[j] = dPopulation[i, j];
                }
                dCostVector[i] = dSingularCost(dTemp);
            }
            for (int i = 0; i < iHalfPopSize; i++)
            {
                for (int j = 0; j < iTournamentSize; j++)
                {
                    iRandom = oRnd.Next(0, iPopSize);
                    Index[j] = iRandom;
                }
 
                for (int j = 1; j < iTournamentSize; j++)
                {
                    if (dCostVector[Index[0]] > dCostVector[Index[j]])
                        Index[0] = Index[j];
                }
                for (int j = 0; j < iChromosomeLength; j++)
                {
                    dMatingPool[i, j] = dPopulation[Index[0],j];
                }
            }
            dPopulation = dMatingPool;
        }


        protected void vSelectionElite()
        {
            double[,] dTemporaryArray = new double[iArraySize, iArraySize];
            int z = 0;
            double dMedian = dMedianMethod(dPopCostSum);
            for (int i = 0; i < iPopSize; i++)
            {
                
                if (dMedian >= dPopCostSum[i])
                {
                    for (int j = 0; j < iChromosomeLength; j++)
                    {
                        dTemporaryArray[z, j] = dPopulation[i, j];
                    }
                    z++;
                }
            }
            dPopulation = dTemporaryArray;
        }


        protected int iMethodHalfPopSize()
        {
            int iHalfPopSize = 0;
            if (1 == iPopSize % 2)
            {
                iHalfPopSize = (iPopSize + 1) / 2;
            }

            if (0 == iPopSize % 2)
            {
                iHalfPopSize = iPopSize / 2;
            }
            return iHalfPopSize;
        }


        protected void vPairing()
        {
            int iHalfPopSize = iMethodHalfPopSize();
            int iParent1, iParent2;
            double dPercent;
            double iTemp1;
            double iTemp2;
            double[] dChild1 = new double[iChromosomeLength];
            double[] dChild2 = new double[iChromosomeLength];
            double[] dParent1 = new double[iChromosomeLength];
            double[] dParent2 = new double[iChromosomeLength];
            double[,] dTemp = dPopulation;
            for (int i = 0; i < iPopSize; i++)
            {
                do
                {
                    iParent1 = oRnd.Next(0, iHalfPopSize);
                    iParent2 = oRnd.Next(0, iHalfPopSize);
                }
                while (iParent1 == iParent2);

                for (int p = 0; p < iChromosomeLength; p++)
                {
                    dParent1[p] = dTemp[iParent1, p];
                }
                for (int p = 0; p < iChromosomeLength; p++)
                {
                    dParent2[p] = dTemp[iParent2, p];
                }
                for (int j = 0; j < iChromosomeLength; j++)
                {

                    iTemp1 = dParent1[j] - alpha * (dParent1[j] - dParent2[j]); 
                    iTemp2 = dParent2[j] + alpha * (dParent1[j] - dParent2[j]);
                    do
                    {
                        dPercent = iRnd.Next(0, 1000000000) / 1000000000.0;
                        dChild1[j] = iTemp1 + dPercent * (iTemp2 - iTemp1);
                    }
                    while (dChild1[j] < iRangeXS || dChild1[j] > iRangeXE);
                }
                if (dSingularCost(dChild1) < dSingularCost(dParent1) && dSingularCost(dChild1) < dSingularCost(dParent2))
                {
                    for (int k = 0; k < iChromosomeLength; k++)
                    {
                        dPopulation[i, k] = dChild1[k];
                    }
                    
                    if (dPopCostSum[0] > dMinimalCost)
                    {
                        for (int w = 0; w < iChromosomeLength; w++)
                        {
                            dPopulation[0, w] = dMinimalSubject[w];
                        }
                    }
                    
                }
                else 
                { 
                    i--; 
                }  
            }
        }
           

     
        protected double dMedianMethod(double[] dVector)
        {
            double[] dSecondArray = new double[iPopSize];
            dSecondArray = dVector;
            Array.Sort(dSecondArray);

            double dMedian = 0;

            if ((iPopSize % 2) == 1)
            {
                int i1 = (iPopSize + 1) / 2 - 1;
                dMedian = dSecondArray[i1];
            }
            if ((iPopSize % 2) == 0)
            {
                int i1 = iPopSize / 2 - 1;
                dMedian = (dSecondArray[i1] + dSecondArray[i1 + 1]) / 2;
            }
            return dMedian;
        }

        protected void vPrintToFile(int iChoose, int iCycle)
        {
            if (iChoose == 0)
            {
                sw.WriteLine("************* POPULACJA STARTOWA *********************");
                sw.WriteLine();
                sw.Write("POLE ZADANE: ");
                for (int x = 0; x < iChromosomeLength; x++)
                {
                    sw.Write("   " + String.Format("{0:N3}", dPopHzRequired[x]) + "   ");
                }

                for (int z = 0; z < iPopSize; z++)
                {
                    sw.WriteLine();
                    sw.Write("osobnik " + (z + 1) + ":");
                    for (int x = 0; x < iChromosomeLength; x++)
                    {
                        sw.Write("    " + String.Format("{0:N3}", dPopHzObtained[z, x]));
                        if (dPopulation[z, x] > 0) sw.Write(" ");
                    }
                }

                sw.WriteLine();
                sw.WriteLine();
            }

            if (iChoose == 1)
            {
                sw.WriteLine("************* CYKL NUMER: " + (iCycle + 1) + "*********************");
                sw.WriteLine();
                sw.Write("POLE ZADANE: ");
                for (int x = 0; x < iChromosomeLength; x++)
                {
                    sw.Write("  " + String.Format("{0:N3}", dPopHzRequired[x]) + "   ");
                }

                for (int z = 0; z < iPopSize; z++)
                {
                    sw.WriteLine();
                    sw.Write("osobnik " + (z + 1) + ":");
                    for (int x = 0; x < iChromosomeLength; x++)
                    {
                        sw.Write("    " + String.Format("{0:N3}", dPopHzObtained[z, x]));
                        if (dPopulation[z, x] > 0) sw.Write(" ");
                    }
                }

                sw.WriteLine();
                sw.WriteLine();
            }

            if (iChoose == 2)
            {
                sw.WriteLine(" Najmniejszy koszt " + dMinimalCost);
                sw.Write("Wartości prądów otrzymane dla optymalnego osobnika: ");
                vPrint1(dMinimalSubject, 1);
            }
            
        }

        protected void vInitialPopulation()
        {
            int iHalfPopSize = iMethodHalfPopSize();
            Random oRnd = new Random();
            for (int i = 0; i < iPopSize; i++)
            {
                for (int y = 0; y < iChromosomeLength; y++)
                {
                    dPopulation[i, y] = oRnd.Next(iRangeXS, iRangeXE);
                }
            }
        }


        protected void vFillA01()
        {
            int ITemp = System.Convert.ToInt16(iChromosomeLength / 2);

            for (int i = 0; i <= iChromosomeLength; i++)
            {
                dA1[i] = (dSingleA * (i - ITemp));
            }

            for (int i = 0; i < iChromosomeLength; i++)
            {
                dA1[i] = (dA1[i]+dA1[i+1])/2;
            }
        }

        protected void vFillKIJMatrix()
        {
            for (int i = 0; i < iChromosomeLength; i++)
            {
                for (int j = 0; j < iChromosomeLength; j++)
                {
                    dKij[i, j] = Math.Pow(dR, 2) / Math.Pow((Math.Sqrt(Math.Pow(dR, 2) + Math.Pow((dA1[i] - dA1[j]), 2))), 3);
                }
            }

        }

        protected void vPrint1(double[] dVector, int ver)
        {
            if(ver == 1)
            {
                sw.WriteLine();
                for (int i = 0; i < iChromosomeLength; i++)
                {
                    sw.Write(String.Format("{0:N3}", dVector[i]) + "    ");  
                }
                sw.WriteLine();
            }
            if (ver == 2)
            {
                sw.WriteLine();
                for (int i = 0; i < iPopSize; i++)
                {
                    sw.Write(String.Format("{0:N3}", dVector[i]) + "    ");
                }
                sw.WriteLine();
            }
        }

        protected void vPrint2(double[,] dVector)
        {
            sw.WriteLine();
            for (int i = 0; i < iPopSize; i++)
            {
                for (int j = 0; j < iChromosomeLength; j++)
                {
                    sw.Write(dVector[i, j] + " ");
                }
                sw.WriteLine();
            }
        }

        public Solenoid(Form1 Form1)
        {
            oMainWindow = Form1;
        }

        public Solenoid()
        {
        }
    }
}