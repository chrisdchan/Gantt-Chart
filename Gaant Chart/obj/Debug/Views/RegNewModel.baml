   M S B A M L   `   `   `  ����F  BGaant Chart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Gaant_ChartGaant_Chart  5   	       Gaant_Chart.RegNewModel   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Gaant_Chart     5   	     � 
ResizeMode$  NoResize=�5       � Title$ RegNewModel��5   	   $	��450��6   $	��400��6*   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��2* �6   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� $��0��6   $��0��6   $��2��5      $��Center=�5      $��Center=�5      $
��Bold"�5      $��25(�6"   $��New Model Registration��5      60   �� 5   
   $��1��6   $��0��6   $��2��5      $��90, 0, 0, 0q�5      $
��Bold"�5       $��20(�6"   $��Model ID��5!      6%   �� 5"   
   -   5        $��	tbModelID��5'      $��1��5"      $��0��6   $��2��5#      $��Center=�5$      $��18(�5%      $��40��5&      $	��200��6   ���#AFDBFF5)   )   �� 5+   
   $��2��6   $��0��6   $��2��5,      $��90, 0, 0, 0q�5-      $
��Bold"�5.      $��20(�6"   $��Start Date/Scan Date��5/      61   (  "System.Windows.Controls.DatePicker  50   
   -   5          Name$ 
datePicker��56      $��2��50      $��0��6"   $��2��51      $��0, 30, 0, 0q�6)   $��Center=�52      $��18(�53      $	��200��54      $��Center=�55      $��45��67   56      �� 59   
   -   5        $��btnCreateModel��5=      $��4��59      $��0��6   $��40��5:      $	��225��6   $��Right=�5;      $��0, 0, 10, 20q�5<      $��Create New Model Tracker��5?      66   �� 5A   
   -   5        $��4��5A      $��1��6   $��40��5B      $��75��6   $
��Left=�5C      $��10, 0, 0, 20q�5D      $��Cancel��5F      6$   5H      5I      