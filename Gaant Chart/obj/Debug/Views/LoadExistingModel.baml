   M S B A M L   `   `   `  ����F  BGaant Chart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Gaant_ChartGaant_Chart  5   	   #    Gaant_Chart.LoadExistingModel   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Gaant_Chart     5   	     � 
ResizeMode$  NoResize=�5       � Title$ LoadExistingModel��5   	   $	��450��6#   $	��350��60   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��3* �6   �� 5      $��1* �6   .+�5   
   �� $��0��6   $
��Bold"�5      $��30(�6"   $��Center=�5      $��Center=�6-   $��Select Existing Model��5      62   �� 5   
   -   5         ��Name$ 	searchTxt��5      $��19(�5      $��1��6    $��50��5      $	��250��6   $��Center=�5      $��Bottom=�5      $��Sample��5      $��0q�5      $��
7, 0, 0, 0q�5      ���#EAEAEA.��5"   '    � 	Resources 5#       �� 
TargetType �� Property �� Value��%�   '��       �� 5$      " ��6   .�5%      ��  �@��CornerRadius$ 10, 10, 0, 0��65   �� 5&       � ��$ 
0, 0, 0, 1q�68   �� 5'       � ��
 �����64   5(      5)      .��6!   5*      �� 5,   
   
�������$	��220��6'   $��5, 0, 20, 7.5q�63   $��1��5-      $��Bottom=�6!   �� 5/   
   $��2��50      $	��267��6   $��16, 0, 0, 0q�6&   $	��200��51      $	��Top=�6   .��52      �� -   5         ��Name$ modelNamesSP��53      $	��250��52      54      55      �� 57   
   -   5        $��Cancel��57      $��3��6"   $	��125��58      $��40��6    5:      ��%^   '��       �� 5;      " ��6   .�5<      ��  �@��CornerRadius$ 3��65   5=      5>      5?      5A      5B      