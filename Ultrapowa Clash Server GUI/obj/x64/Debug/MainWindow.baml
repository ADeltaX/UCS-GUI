   M S B A M L   `   `   `  ����U  QUltrapowa Clash Server GUI, Version=0.6.2.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Ultrapowa_Clash_Server_GUIUltrapowa_Clash_Server_GUI  5   	   O,clr-namespace:Ultrapowa_Clash_Server_GUI.SysUltrapowa_Clash_Server_GUI.Sys  5   	   +    %Ultrapowa_Clash_Server_GUI.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:Ultrapowa_Clash_Server_GUI     5   	   ?custom,clr-namespace:Ultrapowa_Clash_Server_GUI.Sys     5   	   -   5          � Title$  
UCS Server��5	   	   $	��450��6   $	��600��6)   $	��600��5
   	   $	��450��6   ��5      �� 5   
    �� ImageSource$ BG/UCS BG.png��6   .��5      � $��0,0,0,0q�6    � RowDefinitions 5   
   .+�5   
   o� $��0��6   $��	12,12,0,0q�6   
������ .@�5      m� $��_Menu��6   $��22��6=   ���#39c8e2��6&   Z� ��6^   Z� .@�5      m� $��_Restart��6   �� 5      m� 5      -   5        $��_Exit��5      6*   5      �� 5      #��["�6   ���#4aa3df J� Opacity$	 0.8Q�6k   m� 5      $��	_Commands��6   $��22��6*   ���#39c8e2��66   Z� ��6b   Z� .@�5      m� -   5         m�Name$ MI_Ban��5   (   $	��Ban��6   ���	#FF1700FF.@�5      m� $��_Ban IP��6   5      m� 5      $
��Kick��6   ���	#FF0023FF6)   �� 5       m� 5!      $	��Set��6   5"      �� 5#      #��["�6   ���#4aa3df$	 0.8Q�6k   m� 5$      $��_Utility��6   $��22��6)   ���#39c8e2��65   Z� ��6a   Z� 5%      �� 5&      #��["�6   ���#4aa3df$	 0.8Q�6k   m� 5'      $��_Options��6   $��22��6)   ���#39c8e2��65   Z� ��6a   Z� 5(      �� 5)      #��["�6   ���#4aa3df$	 0.8Q�6k   m� 5*      $��_About��6   $��22��6'   ���#39c8e2��63   Z� ��6_   Z� 5+      �� 5,      #��["�6   ���#4aa3df$	 0.8Q�6k   m� 5-      $��	_Feedback��6   $��22��6*   ���#39c8e2��66   Z� ��6b   Z� 5.      �� 5/      #��["�6   ���#4aa3df$	 0.8Q�6k   m� 50      -   5        $ MI_CheckUpdate��50   .   $��_Check Update��6   $��22��6F   ���#39c8e2��6R   Z� ��6~   Z� 51      52      � 53   
   $��1��6   .+�54      �� -   5         ��Name$ label_console��54      $��Console:��6+   $
��Left=�6>   $��	16,40,0,0q�6Y   $	��Top=�6l   $��16��6�   $��0,0,0,0q�6�   
�������6�   �� 55      -   5        $ label_player��55      $��Player online:��6*   $��	0,40,90,0q�6C   $	��Top=�6V   $��16��6n   $��0,0,0,0q�6z   
�������$��Right=�6�   $��71��6�   �� 56      ���#2980b9$��0,65,172,80q�6'   $��1��6<   $��Right=�6F   $	 0.8Q�6b   57      �� 58      ���#2980b9$��1��6'   $��
24,0,24,63q�62   $��Bottom=�6F   $	 0.8Q�6a   59      �� 5:      ���#2980b9$��	12,35,0,0q�6'   $��1��6:   $	��Top=�6E   $
��Left=�6]   $	��449��6x   $	 0.8Q�6�   5;      �� 5<      -   5         ��Name$ RTB_Console��5<   W      FlatRichTextBoxStyle#��[  6   $��2q�6K   $��10,60,180,70q�6�    J� 	AllowDrop .  $��2q�6�    �� 
IsReadOnly . 
������ 	 �� SelectionBrush	 �	#FF005DB9"
 �� VerticalScrollBarVisibility$

 Auto=�6  $��Consolas)�6*  ���#39c8e2. �5=      /� .T�5>      J� .*�6    .*�5?      �� $� ��6   5@      5A      5B      �� 5D      -   5         ��Name$ BTN_LaunchServer��5D        RoundedButton#��[ 6/   $��Launch Server��6V   $��	0,0,15,10q�6n   $��45��6�   $��Bottom=�6�   $��Right=�6�   $	��150��6�   ���#39c8e2$��16(�6  5E      �� 5F      -	   5        $ 	BTN_Enter��5F      #��[ 6(   $��Center=�6O   $��Center=�6s   $	��→��6�   $��
0,0,182,29q�6�   $��26��6�   $��Bottom=�6�   $��Right=�6�   $��28��6�   $��13.333(�6  ���#39c8e25G      �� 5H      -
   5        $ LBL_IP��5H      $��Local IP: 127.0.0.1��6$   $
��Left=�6B   $��10,0,0,1q�6]   $��26��6o   $��Bottom=�6{   $��1q�6�   
�������5I      �� 5J      -   5         ��Name$ CommandLine��5J   d     FlatTextBoxStyle#��[ 6    �� 
CaretBrush �#39c8e2$��4q�6X    �� TextWrapping$
 Wrap=�6y   $�� ��6�   $��12,0,215,33q�6�   $��22��6�   $��Bottom=�6�   ���	#A500697C
����������#39c8e2	 �	#FF1B60A6.��6J  5K      �� 5L      -   5         ��Name$ LB_CommandTypedList��5L      $��12,0,215,55q�63   $��Bottom=�6H   $y�	Collapsed=�6c   6z   �� 5M      -   5         ��Name$ CB_Debug��5M   >     CheckBoxStyle#��[ 6   $��
Debug mode��6P   $��Right=�6e   $��	0,0,179,0q�6�   $��Bottom=�6�   
������� d� 	IsChecked$
 TrueY�6�   ���	#FF2980B96�   �� 5N      -   5        $ listBox��5N      $��Right=�6'   $��
0,60,15,72q�6C   $	��150��6W   ���	#FF39C8E2���	#A500697C$��2q�6�    � 	Resources 5O       �� 
TargetType �� Property �� Value��%^   '��       �� 5P      " ��6   .�5Q      ��  �@��CornerRadius$ 5��69   5R      5S      5T      5U      5V      5W      