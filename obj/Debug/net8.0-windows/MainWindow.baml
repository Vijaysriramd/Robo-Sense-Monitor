   M S B A M L   `   `   `  ����M  IRobo Sense Monitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Robo_Sense_MonitorRobo_Sense_Monitor  5   	   #    Robo_Sense_Monitor.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:Robo_Sense_Monitor     5   	     � Title$  
MainWindow��5   	   $	��600��6   $
��1000��6)    � 	Resources 5	      3  -System.Windows.Media.Effects.DropShadowEffect   DropShadowEffect   
BlurRadius   ShadowDepth   Opacity�� 5
   
   %B   &	          5      $ 10Q�68   $ 3Q�6H   $	 0.3Q�6X   5      .��5      � .+�5   
   ��  �� Orientation$ Vertical=�6   $��Stretch=�6,   $��Stretch=�6J    5       �� MergedDictionaries �� Source  DefaultBackgroundColorBrush �� Color  DefaultBackgroundColor	 �� 
TargetType
 �� Property �� Value�� 5       5      �� 5      $ Styles\StyleResource.xaml?�6-   %~   &	       &	 "     7  �� 5      $ Cornsilk��6J   �� 5      "	 ��6;   .�5      �� 
 � ��8   67   5      5      .+�5       ]� 
������ .+�5!      o� $	��Top=�6   .@�5"      m� $��Options��6   .@�5#      m� -   5        $��MnuLoadConfig��5#   #   $��Load config file��68    m� Command 6R   �� *LoadConfigCommand+m� 5$      -   5        $��MnuSaveConfig��5$   #   $��Save Config file��68    6S   �� *SaveConfigCommand+m� 5%      -   5        $��MnuLoadFromDB��5%   #   $��Load from DB��68    6N   �� *LoadFromDbCommand+m� 5&      -   5        $��MnuSaveTODB��5&   #   $��Save to DB file��66    6O   �� *SaveToDbCommand+5'      m� 5(      $��About��6   .@�5)      m� -   5        $��About��5)   #   $��About RoboSense Monitor��60   5*      5+      5,      �� 5.      $ 
Horizontal=�6   .+�5/      �� -   5        $��lblConnectioninfo��5/        
SenseLabel#��[ 61   $	��Top=�6V   $��
10,20,0,20q�6o   $��Serial Port Connection��6�   $
��Bold"�6�   52      &   System.Windows.Controls.DataGrid  55      -   5          Name$ maingrid��55         AutoGenerateColumns$ False��6�      CanUserAddRows$ False��6�   
������    RowBackground
 ����    HeadersVisibility$ Column=�6     SelectionMode$ Extended=�66  $	��400��6O     	RowHeight$ 30Q�6]     HorizontalGridLinesBrush
 �����   VerticalGridLinesBrush
 �����$��	10,0,10,0q�6�  ��6)   �� *
DataList+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedItem 6|   �� *SelectedDevice+$ TwoWay=� 56      ��%T   (��       57   &   * SystemColors.HighlightBrushKey+)�� 6   $ #cf2e2e��6X   59         ColumnHeaderStyle 5:      �� 5;      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"	  6   .�5<      �� 
 � ��$
 Bold"�67   �� 5=      
 � ��
 ���� 67   �� 5>      
 � ��$ 0,0,1,1q�6<   �� 5?      
 � ��
 �   �68   �� 5@      
 � ��$ Center=�6G   5A         Columns 5D      4  .System.Windows.Controls.DataGridCheckBoxColumn  5E      ,  &System.Windows.Controls.DataGridColumn   Header$ Select��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.5* 5G       1  +System.Windows.Controls.DataGridBoundColumn   Binding 5F       �� *
IsSelected+$ TwoWay=�5G       4  .System.Windows.Controls.DataGridTemplateColumn  5H      $ ID��6-     columnDeviceId   CellTemplate# [ 69   $
 0.5* 6h   5I        5K      $ Device Name��6-     columnDeviceName# [ 6B   $ * 6s   5L        5N      $ 	Port Name��6-     columnPortName# [ 6@   $
 0.7* 6o      CellEditingTemplate# [ 6|   5O        5Q      $ 	baud Rate��6-     columnbaudRate# [ 6@   $
 0.6* 6o   # [ 6|   5R        5U      $ DataBits��6-     columnDataBits# [ 6?   $
 0.5* 6n   5V        5Y      $ Parity��6-    	 columnParity# [	 6=   $
 0.5* 6j   5Z        5\      $ StopBits��6-    
 columnStopBits# [
 6?   $
 0.8* 6n   5]        5_      $ Status��6-     columnConnectionStatus# [ 6=   $
 0.6* 6t   5`        5b      $ 
Delete Row��6-     columnDeleteRow# [ 6A   $ * 6q   5c        5e      $ Information��6-     columnInformation# [ 6B   $ 0.75* 6t   5g        5h      $ 	ShowPlots��6-     columnPlots# [ 6@   $ 0.75* 6n   5i        5j      $ 	Play Back��6-     columnPlayback# [ 6@   $ 0.75* 6o   5k      5u      �� 5w      $��50��6   $ 
Horizontal=�6&   .+�5z      �� -   5        $��btnReset��5z      $��20,10,10,10q�6)   $��80��6>   $��Reset��6I     SenseButton#��[ 6Y   ��6~   �� *ResetCommand+�� 5{      -	   5        $��	btnAddRow��5{      $��20,10,10,10q�6*   $��80��6?   $��Add Row��6K   #��[ 6�   ��6]   �� *AddRowCommand+6�   	� 5|      -
   5        $��rBtnParallel��5|   3     	� 	GroupName$  Parallel��6     SenseRadioButton#��[ 6G   $��20,15,20,10q�6q   $y�Hidden=�6�   .��6�   
Parallel6�   	� 5}      -   5        $��rBtnSequential��5}   3   $  Parallel��6   #��[ 6I   $��
0,15,20,10q�6s   $y�Hidden=�6�   .��6�   
Sequential6�   �� 5~      -   5        $��ShowAllPlots��5~      $��100,10,20,0q�6-   $��Show All Plots��6B   #��[ 6�   ��6\   �� *ShowAllPlotCommand+6�   �� 5      -   5        $��btnStartMonitor��5      $��
00,10,20,0q�60   $��Start Monitor��6D   #��[ 5�      ��5   ]   �� *StartMonitorCommand+5�      �� 5�      -   5        $��btnEndMonitor��5�      $��
00,10,20,0q�6.   $��End Monitor��6B   #��[ 5�      ��5�   Y   �� *EndMonitorCommand+5�      5�      5�      � 5�   
   $��Center=�5�      ! � 	StaysOpen$! False��5�      ��5�      �� *IsThresholdPopupVisible+.��5�      �� 
�������" �� Padding$" 20q�5�      ���	#FF0078D7$��2q�5�      # �� CornerRadius$# 5��5�      $ J� Effect#$ [  5�      ._�5�      �� .+�5�      �� $$��Today's Threshold Distribution��6    $��16(�5�      $
��Bold"�6*   $��0,0,0,10q�6<   � 5�      % � RowDefinitions% 5�      �� 5�      $
��Auto �6,   �� 5�      $
��Auto �6,   �� 5�      $
��Auto �6,   �� 5�      $
��Auto �6,   & � ColumnDefinitions& 5�      �� 5�      $
��Auto �6/   �� 5�      $��* �6/   .+�5�      �� $��0��6%   $��0��65   $ 
Horizontal=�6B   $	��0,5q�6[   .+�5�      �� $��15��6(   $��15��63   
���  ��$��0,0,5,0q�6J   �� 5�      $��Above Threshold:��6(   $��SemiBold"�6@   5�      �� 5�      $��1��6$   $��0��64   $
��10,5q�5�       $��Right=�6.   ��5�   A   �� *'%CurrentThresholdStats.AbovePercentage+' �� StringFormat$' {0:F1}%��5�   .   �� 5�      $��0��6%   $ 
Horizontal=�66   $	��0,5q�6O   $��1��6\   .+�5�      �� $��15��6(   $��15��63   
��� � �$��0,0,5,0q�6L   �� 5�      $��Normal Range:��6(   $��SemiBold"�6=   5�      �� 5�      $��1��6$   $
��10,5q�5�       $��Right=�6.   $��1��6J   ��5�   4   �� *(&CurrentThresholdStats.NormalPercentage+$' {0:F1}%��5�   J   �� 5�      $��0��6%   $ 
Horizontal=�65   $	��0,5q�6N   $��2��6[   .+�5�      �� $��15��6(   $��15��63   
����  �$��0,0,5,0q�6K   �� 5�      $��Below Threshold:��6(   $��SemiBold"�6@   5�      �� 5�      $��1��6$   $
��10,5q�5�       $��Right=�6.   $��2��6J   ��5�   4   �� *'%CurrentThresholdStats.BelowPercentage+$' {0:F1}%��5�   J   5�      �� 5�      $��Close��6   $��Right=�5�      $��0,10,0,0q�65   $��80��6G   ��5�   -   �� *CloseThresholdPopupCommand+5�   G   5�      5�      5�      5�      5�      