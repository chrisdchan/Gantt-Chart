   M S B A M L   `   `   `  ����F  BGaant Chart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Gaant_ChartGaant_Chart  5   	       Gaant_Chart.CompleteTask   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Gaant_Chart     5   	     � 
ResizeMode$  NoResize=�5       � Title$ CompleteTask��5   	   $	��300��6   $	��300��6+   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   �� 5      $
��0.5* �6   �� 5      $��1* �6   �� 5      $��0.75* �6   .+�5   
   �� $��0��6   $	��200��5      $��Center=�5      $��25(�5      $
��Bold"�6"   $
��Wrap=�5       �� TextAlignment$ Center=�6(   $��Document Completed Task��5      �� 5   
   -   5         ��Name$ assignedTxtBlk��5      $��Center=�6,   $��1��5      $ Center=�6"   $��16(�5      $��30��6"   �� 5   
   $��2��6   $
��Date��6   $��30��6,   $��Center=�68   $��16(�5      $��18, 0, 0, 50q�5      (  "System.Windows.Controls.DatePicker  5   
   -   5        $��
datePicker��5      $��Center=�6'   $��Center=�6I   $��16(�5      $��2��5      $	��130��6"   $��30��6.   $
��Left=�6:   $��10, 30, 0, 0q�6U   �� 5    
   $
��Time��6   $��2��6   $��Center=�6,   $��30��5!      $��16(�6   $��150, 0, 0, 50q�5"      �� 5#   
   -   5         ��Name$ hoursTxt��5#   6   $��2��6   $��30��6   $��30��6+   $��11��6H   $��16(�5$      $��50, 30, 0, 0q�5%       �� TextAlignment$ Center=�6(   $��Center=�6?   �� 5&   
   $��:��6   $��2��6   $��93, 30, 0, 0q�6)   $��15��6?   $��30��6J   $��Center=�5'      �� 5(   
   -   5        $ 
minutesTxt��5(   6   $��2��6   $��30��6   $��30��6+   $��130, 30, 0, 0q�5)      $��59��6)   $ Center=�63   $��Center=�6J   �� 5*   
   -   5         ��Name$ timeComboBox��5*   +   $��2��6   $��45��6    $��220, 30, 0, 0q�5+      $��30��6*   .@�5,      �� $��AM��6   �� 5-      $��PM��6    �� 
IsSelected . 6(   5.      �� 50   
   -   5        	 ��Name$	 	submitBtn��50      $��3��51      $��40��52      $	��100��6   $
��Left=�53      $��50, 0, 0, 0q�54      $��Submit��55      $��15(�6"   ���#00B6FF
�������$��0q�57      �� 59   
   -   5        $	 	cancelBtn��59      $��3��5:      $��40��5;      $��75��6   $��Right=�5<      $��0, 0, 50, 0q�5=      $��15(�5>      $��Cancel��6   5@      5A      