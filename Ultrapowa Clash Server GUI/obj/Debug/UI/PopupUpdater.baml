   M S B A M L   `   `   `  ����>  :UCS, Version=0.6.5.0, Culture=neutral, PublicKeyToken=nullclr-namespace:UCS.UIUCS.UI  5   	       UCS.UI.PopupUpdater   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:UCS.UI     5   	   -   5          � Title$  Popup��5   	   $	��300��6   $	��400��6$    � Topmost .  � WindowStyle$
 None=�6?   ���#171717 � AllowsTransparency .  � 
ResizeMode$ NoResize=�6�    � WindowStartupLocation$ CenterOwner=�6�    � ShowInTaskbar .  .��5	      �� -   5        $��windowBorder��5	      $��1q�6!   ���	#ff00779f._�5
   
   � .+�5      �� ���	#ff00779f$��30��6)   $	��Top=�65    J� RenderTransformOrigin$ 0.5,0.5&�6M   �� 5      -   5         ��Name$ 
btn_Cancel��5   <      RoundedButton#��[  6   $��Close��6P   $
��Left=�6`   $��225,263,0,0q�6{   $	��Top=�6�   $��79��6�   $��25��6�   $��3q�6�   ���	#FF262626
�������$��Segoe UI Light)�6  5      �� 5      -   5        $ 
btn_GoPage��5   <   #��[  6   $��Download��6P   $
��Left=�6c   $��309,263,0,0q�6~   $	��Top=�6�   $��79��6�   $��25��6�   ���	#FF262626
�������$��Segoe UI Light)�6  5      �� 5      -   5        	 ��Name$	 	lbl_Title��5      $(��"A new version of UCS is available!��6'   $
��Left=�6T   $��29,-2.28,0,0q�6o   $	��Top=�6�   
�������$	��369��6�   $��16(�6�   $��Segoe UI Light)�6�   5      �� 5      -   5        
 ��Name$
 image��5      $
��Left=�6#   $��30��6>   $��Images/Commands.png��6J   $	��Top=�6g   $��30��6   $��	-1,-1,0,0q�6�   5      �� 5      -   5        $	 
lbl_CurVer��5      $��Current UCS version: ��6(   $
��Left=�6H   $��	10,40,0,0q�6c   $	��Top=�6v   
�������$	��182��6�   $��Segoe UI Light)�6�   5      �� 5      -   5        $	 
lbl_NewVer��5      $��New UCS version: ��6(   $��	0,40,10,0q�6D   $��Right=�6W   $	��Top=�6z   
�������$��Right=�6�   $	��177��6�   $��Segoe UI Light)�6�   5      �� 5      -	   5        $	 lbl_Changelog��5      $��
Changelog:��6+   $
��Left=�6@   $��	10,81,0,0q�6[   $	��Top=�6n   
�������$��Segoe UI Light)�6�   5      �� 5      -
   5         ��Name$ RTB_Console��5   W     FlatRichTextBoxStyle#��[ 6   $��2q�6K   $��
8,107,8,40q�6�    J� 	AllowDrop .  $��1q�6�    �� 
IsReadOnly . ���	#FF080808 �� SelectionBrush �	#FF005DB9" �� VerticalScrollBarVisibility$
 Auto=�6  $��Consolas)�6&  ���#39c8e2
�������. �5      /� .T�5      J� .*�6    .*�5      �� $� ��6   5      5       5!      5#      5$      5%      