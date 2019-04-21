using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genetic_Algorithms 
{
   public class Test_Functions 
    {
       Form1 oMainWindow;
       double[,] dPopulation = new double[1000,3];
       int iPopSize, iRangeYS, iRangeYE, iRangeXS, iRangeXE, iCycleNumber, iCoefficientOfMutation, iMutationPower, iTournamentSize = 5;
       StreamWriter sw = new StreamWriter("Test_log.txt");
       double alpha = 0.7;
       double[] dMinimalSubject = new double[3];


       public void vMain()
       {
           iPopSize = oMainWindow.iTextBoxToInt(1);
           iRangeYS = oMainWindow.iTextBoxToInt(4);
           iRangeYE = oMainWindow.iTextBoxToInt(8);
           iRangeXS = oMainWindow.iTextBoxToInt(5);
           iRangeXE = oMainWindow.iTextBoxToInt(6);
           iCycleNumber = oMainWindow.iTextBoxToInt(3);
           iCoefficientOfMutation = oMainWindow.iTextBoxToInt(2);
           iMutationPower = oMainWindow.iTextBoxToInt(7);
           for (int p = 0; p < 3; p++)
           {
               dMinimalSubject[p] = 200;
           }
           Bitmap bFunction = null;
           if (oMainWindow.checkBox3.Checked) bFunction = vDrawDiagram();

           vRandomGenerator();
           vPrintToFile(0, 0);

           for (int i = 0; i < iCycleNumber; i++)
           {
               vCost();

               if (oMainWindow.radioButton5.Checked) vSelectionTournament();
               if (oMainWindow.radioButton6.Checked) vSelectionElite();
               if (oMainWindow.radioButton11.Checked) vSelectionRoulette();
               vPairing();
               vMutation();
               if (oMainWindow.checkBox3.Checked) bFunction = bDrawPoints(bFunction);

               vPrintToFile(1, i);
               sw.WriteLine("");
               sw.WriteLine("Optymalny:" + dMinimalSubject[0] + " " + dMinimalSubject[1] + " " + dMinimalSubject[2]);
           }
           sw.WriteLine();
           if (oMainWindow.checkBox1.Checked) System.Diagnostics.Process.Start("Test_log.txt");
           sw.Close();

           if (oMainWindow.checkBox3.Checked)
           {
               bFunction = bDrawScale(bFunction);
               Form2 oForm2 = new Form2();
               oForm2.pictureBox1.Image = bFunction;
               oForm2.Show();
           }

       }

       protected void vPrintToFile(int iChoose, int iCycle)
       {
          if(iChoose == 0)
          {
             sw.WriteLine("************* POPULACJA STARTOWA *********************");
          }
          if (iChoose == 1)
          {
              sw.WriteLine("************* CYKL NUMER: " + (iCycle + 1) + "*********************");
          }

          for (int z = 0; z < iPopSize; z++)
          {
              sw.WriteLine("osobnik " + (z+1) + ": x:" + dPopulation[z, 0] + " y: " + dPopulation[z, 1] + " z: " + dUnitaryCost(dPopulation[z, 0], dPopulation[z, 1]));
          }
           
       }

       protected void vRandomGenerator()
       {
           Random oRnd = new Random();
           for (int i = 0; i < iPopSize; i++)
           {
               dPopulation[i, 0] = oRnd.Next(iRangeXS, iRangeXE) + oRnd.NextDouble();
               dPopulation[i, 1] = oRnd.Next(iRangeYS, iRangeYE) + oRnd.NextDouble();
           }

          
       }


       protected void vCost()
       {
           for (int i = 0; i < iPopSize; i++)
           {
               dPopulation[i, 2] = dUnitaryCost(dPopulation[i, 0], dPopulation[i, 1]);
               if (dMinimalSubject[2] > dPopulation[i, 2])
               {
                   dMinimalSubject[0] = dPopulation[i, 0];
                   dMinimalSubject[1] = dPopulation[i, 1];
                   dMinimalSubject[2] = dPopulation[i, 2];
               }
           }
       }

       protected double dUnitaryCost(double dX, double dY)
       {
           double dSecondVariable = 0, dReturn = 0, pi = 3.14159265359;
           if (oMainWindow.radioButton1.Checked)
           { 
               dSecondVariable = (dY - Math.Pow(dX, 2));
               dReturn = Math.Pow((1 - dX), 2) + 100 * (Math.Pow(dSecondVariable, 2));
           }

           if (oMainWindow.radioButton2.Checked)
           {
               dReturn = Math.Pow((dX), 2) + Math.Pow((dY), 2);
           }

           if (oMainWindow.radioButton3.Checked)
           {
               dReturn = Math.Pow((dX - 3), 2) + Math.Pow((dY + 5), 2) - 5;
           }

           if (oMainWindow.radioButton4.Checked)
           {
               dReturn = Math.Pow(Math.Sin(3 * pi * dX), 2) + Math.Pow(dX - 1, 2) * (1 + Math.Pow(Math.Sin(3 * pi * dY),2)+ Math.Pow((dY - 1),2)* (1 + Math.Pow((2*pi*dY),2)));
           }
           return dReturn;
       }


       protected void vSelectionTournament()
       {
           Random oRnd = new Random();
           double[,] dMatingPool = new double[1000, 3];
           int iRandom;
           int iHalfPopSize = iMethodHalfPopSize();
           int[] Index = new int[iTournamentSize];
 
           for (int i = 0; i < iHalfPopSize; i++)
           {
               for (int j = 0; j < iTournamentSize; j++)
               {
                   iRandom = oRnd.Next(0, iPopSize);
                   Index[j] = iRandom;
               }
               for (int j = 1; j < iTournamentSize; j++)
               {
                   if (dPopulation[Index[0],2] > dPopulation[Index[j],2])
                       Index[0] = Index[j];
               }
               for (int j = 0; j < 3; j++)
               {
                   dMatingPool[i, j] = dPopulation[Index[0], j];
               }
           }
           dPopulation = dMatingPool;
       }

       protected void vSelectionRoulette()
       {
           Random oRnd = new Random();
           double[,] dTemporaryArray = new double[1000, 3];
           double dSum = 0, dSumTemp = 0, dRandom, dMin = 0, dMax = 0;
           int iHalfPopSize = iMethodHalfPopSize();

           for (int i = 0; i < iPopSize; i++)
           {
               if (dMin > dPopulation[i, 2])
               {
                   dMin = dPopulation[i, 2];
               }
           }

           if (0 > dMin)
           {
               for (int i = 0; i < iPopSize; i++)
               {
                   dPopulation[i, 2] += Math.Abs(dMin);
               }
           }

           for (int i = 0; i < iPopSize; i++)
           {
               dSum += dPopulation[i, 2];

               if (dMax < dPopulation[i, 2])
               {
                   dMax = dPopulation[i, 2];
               }
           }

           for (int i = 0; i < iPopSize; i++)
           {
               dPopulation[i, 2] = dMax - dPopulation[i, 2] + 10;
           }

           for (int z = 0; z < iHalfPopSize; z++)
           {
               dRandom = oRnd.NextDouble() * dSum;
               dSumTemp = 0;
               for (int i = 0; i < iPopSize; i++)
               {
                   dSumTemp += dPopulation[i, 2];

                   if (dSumTemp > dRandom)
                   {
                       dTemporaryArray[z, 0] = dPopulation[i, 0];
                       dTemporaryArray[z, 1] = dPopulation[i, 1];
                       dTemporaryArray[z, 2] = dPopulation[i, 2];
                       i = iPopSize + 1;
                   }
               }
           }
           dPopulation = dTemporaryArray;
       }


       protected void vSelectionElite()
       {
           double dMedian = dMedianMethod();
           double[,] dTemporaryArray = new double[1000, 3];
           int z = 0;
           for (int i = 0; i < iPopSize; i++)
           {
               if (dMedian >= dPopulation[i, 2])
               {
                   dTemporaryArray[z, 0] = dPopulation[i, 0];
                   dTemporaryArray[z, 1] = dPopulation[i, 1];
                   dTemporaryArray[z, 2] = dPopulation[i, 2];
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
           double e1, e2, dPercent, dTemp;
           double[,] dChildrenArray = new double[1000, 3];
           Random oRnd = new Random();

           for (int i = 0; i < iPopSize; i++)
           {
               do
               {
                   iParent1 = oRnd.Next(0, iHalfPopSize);
                   iParent2 = oRnd.Next(0, iHalfPopSize);
               }
               while (iParent1 == iParent2);

               do
               {
                   dPercent = oRnd.NextDouble();
                   e1 = dPopulation[iParent1, 0] - alpha * (dPopulation[iParent2, 0] - dPopulation[iParent1, 0]);
                   e2 = dPopulation[iParent2, 0] + alpha * (dPopulation[iParent2, 0] - dPopulation[iParent1, 0]);

                   dTemp = e1 + dPercent * (e2 - e1);
               }
               while (dTemp < iRangeXS || dTemp > iRangeXE);
               dChildrenArray[i, 0] = dTemp;
               do
               {
                   dPercent = oRnd.NextDouble();
                   e1 = dPopulation[iParent1, 1] - alpha * (dPopulation[iParent2, 1] - dPopulation[iParent1, 1]);
                   e2 = dPopulation[iParent2, 1] + alpha * (dPopulation[iParent2, 1] - dPopulation[iParent1, 1]);

                   dTemp = e1 + dPercent * (e2 - e1);
               }
               while (dTemp < iRangeYS || dTemp > iRangeYE);
               dChildrenArray[i, 1] = dTemp;

           }

           dPopulation = dChildrenArray;
       }

       protected void vMutation()
       {
           Random oRnd = new Random();

           for (int i = 0; i < iPopSize; i++)
           {
               double dNewValue, dRandom;
               if (iCoefficientOfMutation > oRnd.Next(0,100))
               {
                   do
                   {
                    dRandom = oRnd.Next(-iMutationPower, iMutationPower);
                    dNewValue = (dRandom / 100) * dPopulation[i, 0];
                   }
                   while (dNewValue < iRangeXS || dNewValue > iRangeXE);
                   dPopulation[i, 0] += dNewValue;
               }

                if (iCoefficientOfMutation > oRnd.Next(0,100))
               {
                    do
                    {
                   dRandom = oRnd.Next(-iMutationPower, iMutationPower);
                   dNewValue = (dRandom /100) * dPopulation[i,1];
                    }
                    while (dNewValue < iRangeYS || dNewValue > iRangeYE);
                    dPopulation[i, 1] += dNewValue;
               }
           }

       }


       protected double dMedianMethod()
       {
           double[] fSecondArray = new double[iPopSize];
           for (int i = 0; i < iPopSize; i++)
           {
               fSecondArray[i] = dPopulation[i, 2];
           }
           Array.Sort(fSecondArray);
           double dMedian = 0;

           if (1 == iPopSize % 2)
           {
               dMedian = fSecondArray[(iPopSize + 1) / 2 - 1];
           }

           if (0 == iPopSize % 2)
           {
               dMedian = (fSecondArray[iPopSize/2 - 1] + fSecondArray[iPopSize/2]) / 2;
               
           }
           return dMedian;
       }

       protected Bitmap vDrawDiagram()
       {
           int isize = 1000;
           Bitmap bmp = new Bitmap(isize, isize);
           double dMax = dUnitaryCost(iRangeXS, iRangeYS), dMin = dUnitaryCost(iRangeXS, iRangeYS);
           double YStep = (Math.Abs(iRangeYS) + Math.Abs(iRangeYE)) / System.Convert.ToDouble(isize); //zabezpieczyc przed dodatnim S
           double XStep = (Math.Abs(iRangeXS) + Math.Abs(iRangeXE)) / System.Convert.ToDouble(isize);
           double dCostOfStep, dPercentOfMax = 0;

           for (int i = 0; i < isize; i++)
           {
               for (int z = 0; z < isize; z++)
               {
                   dCostOfStep = dUnitaryCost(iRangeXS + (z * XStep), iRangeYS + (i * YStep));

                   if (dMax < dCostOfStep)
                  {
                      dMax = dCostOfStep;
                  }
                   if (dMin > dCostOfStep)
                  {
                      dMin = dCostOfStep;
                  }
               }
           }

           for (int i = 1; i < isize; i++)
           {
               for (int z = 1; z < isize; z++)
               {
                   if (dMin >= 0)
                   {
                       dPercentOfMax = ((dUnitaryCost(iRangeXS + (z * XStep), iRangeYS +((isize - i) * YStep))) / dMax) * 100;
                   }
                   if (dMin < 0)
                   {
                       dPercentOfMax = (((dUnitaryCost(z * XStep, (isize - i) * YStep))+ Math.Abs(dMin)) / (dMax+ Math.Abs(dMin)))*100;
                   }



                   if (dPercentOfMax >= 0 && dPercentOfMax <= 0.2)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 40, 1, 1));
                   }
                   if (dPercentOfMax >= 0.2 && dPercentOfMax <= 1)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 60, 1, 1));
                   }
                   if (dPercentOfMax >= 1 && dPercentOfMax <= 3)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 80, 1, 1));
                   }
                   if (dPercentOfMax >= 3 && dPercentOfMax <= 10)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 120, 1, 1));
                   }
                   if (dPercentOfMax > 10 && dPercentOfMax <= 20)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 170, 1, 1));
                   }
                   if (dPercentOfMax > 20 && dPercentOfMax <= 30)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 255, 1, 1));
                   }
                   if (dPercentOfMax > 30 && dPercentOfMax <= 40)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 255, 153, 1));
                   }
                   if (dPercentOfMax > 40 && dPercentOfMax <= 50)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 153, 153, 1));
                   }
                   if (dPercentOfMax > 50 && dPercentOfMax <= 60)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 1, 153, 1));
                   }
                   if (dPercentOfMax > 60 && dPercentOfMax <= 70)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 77, 204, 77));
                   }
                   if (dPercentOfMax > 70 && dPercentOfMax <= 80)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 77, 204, 153));
                   }
                   if (dPercentOfMax > 80 && dPercentOfMax <= 90)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 1, 77, 204));
                   }
                   if (dPercentOfMax > 90 && dPercentOfMax <= 100)
                   {
                       bmp.SetPixel(z, isize - i, Color.FromArgb(255, 1, 1, 255));
                   }
                  
               }
           }

           return bmp;
       }

       protected Bitmap bDrawPoints(Bitmap bmp)
       {
           double dSumOfY = (Math.Abs(iRangeYS) + Math.Abs(iRangeYE));
           double dSumOfX = (Math.Abs(iRangeXS) + Math.Abs(iRangeXE));
           double dX, dY, dSize = 1000;
           int iX, iY;

           for (int i = 0; i < iPopSize; i++)
           {

               dX = ((dPopulation[i, 0] + Math.Abs(iRangeXS)) * dSize) / (dSumOfX);
               dY = dSize - ((dPopulation[i, 1] + Math.Abs(iRangeXS)) * dSize) / (dSumOfY);
               iX = Convert.ToInt32(Math.Round(dX));
               iY = Convert.ToInt32(Math.Round(dY));

               
               bmp.SetPixel(iX, iY, Color.FromArgb(100, 1, 1, 1));
               bmp.SetPixel(iX -1, iY, Color.FromArgb(255, 255, 255, 255));
               bmp.SetPixel(iX + 1, iY, Color.FromArgb(255, 255, 255, 255));
               bmp.SetPixel(iX, iY-1, Color.FromArgb(255, 255, 255, 255));
               bmp.SetPixel(iX, iY+1, Color.FromArgb(255, 255, 255, 255));

           }
           return bmp;
       }

       protected Bitmap bDrawScale(Bitmap bmp)
       {
           int iSize = 1000;

           for (int i = 10; i < iSize - 7; i++)
           {
               bmp.SetPixel(i,4, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i,5, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, 6, Color.FromArgb(255, 1, 1, 1));
           }

           for (int i = 7; i < 15; i++)
           {
               bmp.SetPixel(10, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(11, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(12, i, Color.FromArgb(255, 1, 1, 1));

               bmp.SetPixel(iSize - 7, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(iSize - 8, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(iSize - 9, i, Color.FromArgb(255, 1, 1, 1));

               bmp.SetPixel(499, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(500, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(501, i, Color.FromArgb(255, 1, 1, 1));
           }

           for (int i = 12; i < iSize - 7; i++)
           {
               bmp.SetPixel(4, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(5, i, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(6, i, Color.FromArgb(255, 1, 1, 1)); 
           }

           for (int i = 7; i < 13; i++)
           {
               bmp.SetPixel(i, 12, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, 13, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, 14, Color.FromArgb(255, 1, 1, 1));

               bmp.SetPixel(i, iSize - 7, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, iSize - 8, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, iSize - 9, Color.FromArgb(255, 1, 1, 1));

               bmp.SetPixel(i, 499, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, 500, Color.FromArgb(255, 1, 1, 1));
               bmp.SetPixel(i, 501, Color.FromArgb(255, 1, 1, 1));
           }

           return bmp;
       }


       public Test_Functions(Form1 Form1)
       {
           oMainWindow = Form1;
       }

       public Test_Functions()
       {
       }
       
    }
}
