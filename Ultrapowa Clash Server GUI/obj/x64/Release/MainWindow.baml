   M S B A M L   `   `   `  ���� >  :UCS, Version=0.6.3.2, Culture=neutral, PublicKeyToken=nullclr-namespace:UCSUCS  !clr-namespace:UCS.SysUCS.Sys      UCS.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:UCS     (customclr-namespace:UCS.Sys     -     � Title$  
UCS Server��$	��600��$	��850��$	��600��$	��450������  �� ImageSource$ BG/UCS BG.png��.��� $��0,0,0,0q� � RowDefinitions .+�o� -    o�Name$ MI_GRID��$��0��$��	12,12,0,0q�
������ .@�m� -    m�Name$ MI_Menu��$��_Menu��$��22�����#39c8e2��Z� ��Z� .@�m� -   $��_Restart��
���   ��� 
���   �
���   �m� -   $��_Exit��
���   ���Z� �� -    ��Name$ SEP_1��#��["����#4aa3df J� Opacity$	 0.8Q�m� -   $ 
MI_Comands��$��	_Commands��$��22�����#39c8e2��Z� ��Z� .@�m� $	��Ban��
���   �.@�m� -   $	��Ban��m� -	   $��Ban IP��m� -
   $��Temporary ban��m� -   $��Temporary ban IP��m� $��Unban��
���   �.@�m� -   $��Unban��m� -   $��Unban IP��m� -   $ MI_Kick��$
��Kick��
���   �m� -   $ MI_Mute��$
��Mute��
���   �.@�m� -   $
��Mute��m� -   $��Unmute���� 
���   �
���   �m� $	��Set��
���   ��� -   $ SEP_2��#��["����#4aa3df$	 0.8Q�m� -   $ 
MI_Utility��$��_Utility��$��22�����#39c8e2��Z� ��Z� .@�m� -   $��Configuration��
���   ��� -   $ SEP_3��#��["����#4aa3df$	 0.8Q�m� -   $ 
MI_Options��$��_Options��$��22�����#39c8e2��Z� ��Z� �� -   $ SEP_4��#��["����#4aa3df$	 0.8Q�m� -   $ MI_About��$��_About��$��22�����#39c8e2��Z� ��Z� �� -   $ SEP_5��#��["����#4aa3df$	 0.8Q�m� -   $ MI_Feedback��$��	_Feedback��$��22�����#39c8e2��Z� ��Z� �� -   $ SEP_6��#��["����#4aa3df$	 0.8Q�m� -   $ MI_CheckUpdate��$��_Check Update��$��22�����#39c8e2��Z� ��Z� � $��1��.+��� -    ��Name$ label_console��$��Console:��$
��Left=�$��	16,40,0,0q�$	��Top=�$��16��$��0,0,0,0q�
��������� -   $ label_player��$ ��Player online: (Name : ID)��$��	0,40,24,0q�$	��Top=�$��16��$��0,0,0,0q�
�������$��Right=�$	��137���� ���#2980b9$��0,65,173,80q�$��1��$��Right=�$	 0.8Q��� ���#2980b9$��1��$��
24,0,24,63q�$��Bottom=�$	 0.8Q��� ���#2980b9$��	12,35,0,0q�$��1��$	��Top=�$
��Left=�$	��580��$	 0.8Q� J� RenderTransformOrigin$ 0.5,0.5&��� -   	 ��Name$	 RTB_Console��   FlatRichTextBoxStyle#��[  $��2q�$��12,60,182,70q�
 J� 	AllowDrop
 .  $��2q� �� 
IsReadOnly . 
������  �� SelectionBrush �	#FF005DB9" �� VerticalScrollBarVisibility$
 Auto=�$��Consolas)����#39c8e2. �/� .T�J� .*�.*��� $� ���� -     ��Name$ BTN_LaunchServer��  RoundedButton#��[ $��Launch Server��$��	0,0,15,10q�$��45��$��Bottom=�$��Right=�$	��150�����#39c8e2$��16(��� -!   $ 	BTN_Enter��#��[ $��Center=�$��Center=�$	��→��$��
0,0,182,29q�$��26��$��Bottom=�$��Right=�$��28��$��13.333(����#39c8e2�� -"   $ LBL_IP��$��Local IP: 127.0.0.1��$
��Left=�$��10,0,0,0q�$��27��$��Bottom=�$��1q�
��������� -#   $ LBL_PORT��$��
PORT: 9339��$
��Left=�$��	150,0,0,1q�$��26��$��Bottom=�$��1q�
��������� -$   $ 
LBL_UpTime��$��Up time: 00:00:00��$��	0,0,282,0q�$��28��$��Bottom=�$��1q�
�������$��Right=�$	��Top=�$��Right=�$	��143���� -%    ��Name$ CommandLine��  FlatTextBoxStyle#��[  �� 
CaretBrush �#39c8e2$��4q� �� TextWrapping$
 Wrap=�$�� ��$��12,0,215,33q�$��22��$��Bottom=����	#A500697C
����������#39c8e2 �	#FF1B60A6.���� -&    ��Name$ LB_CommandTypedList��$��12,0,215,55q�$��Bottom=�$y�	Collapsed=����	#FF39C8E2���	#D800697C$��1q�
������� � 	Resources  �� 
TargetType �� Property �� Value��%9   '��       �� " ��.���  �@��CornerRadius$ 5���� -'    ��Name$ CB_Debug��  CheckBoxStyle#��[ $��
Debug mode��$��Right=�$��	0,0,179,0q�$��Bottom=�
������� d� 	IsChecked$
 TrueY����	#FF2980B9�� -(   $ listBox��$��Right=�$��
0,60,15,72q�$	��150�����	#FF39C8E2���	#A500697C$��2q�
������� ��%9   '��       �� " ��.���  �@��CornerRadius$ 5��� $
��Left=�$��7��$	��Top=�$	��842��$ 0.5,0.5&�
���  �� � Value$ 0Q�$y�Hidden=�z�V� .~���  �� ScaleY$ -1Q��� �� T� 