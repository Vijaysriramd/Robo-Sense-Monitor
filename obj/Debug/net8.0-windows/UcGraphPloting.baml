   M S B A M L   `   `   `  ����M  IRobo Sense Monitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Robo_Sense_MonitorRobo_Sense_Monitor  5      K*clr-namespace:Robo_Sense_Monitor.ConverterRobo_Sense_Monitor.Converter  5      '    !Robo_Sense_Monitor.UcGraphPloting   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2local clr-namespace:Robo_Sense_Monitor     5      A
converters*clr-namespace:Robo_Sense_Monitor.Converter     5      l hDotNetProjects.Wpf.Extended.Toolkit, Version=5.0.124.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5	        � 	Resources  5         BoolToVisConverter8   2Robo_Sense_Monitor.Converter.ImportParamsConverter  ImportParamsConverter8   2Robo_Sense_Monitor.Converter.ExportParamsConverter  ExportParamsConverter:   4Robo_Sense_Monitor.Converter.PlaybackParamsConverter  PlaybackParamsConverter �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ Styles\StyleResource.xaml?�6%   %t   &	        &	      &	 &     &	 9     �� 5      6+     5      6/     5      6/     5      61   5      .��5      � -   5         �Name$ maingrid��5      $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5    
   �� 5!      $
��Auto �6   �� 5"      $
��Auto �6   .+�5&   
   ��  �� Orientation$ 
Horizontal=�6   $��0 0 0 10q�6.   $��0��6@   $��0��6M   $
��Left=�6]   $	��Top=�6x   .+�5'      �� ��6   �� *DeviceInformation+6=   5(      �� 5)   
   $ 
Horizontal=�6   $��0 0 0 10q�6.   $��1��6@   $��0��6M   $
��Left=�5*      $	��Top=�6(   y�5+      �� *IsPlaybackMode+ �� 	Converter# [  .+�5.      �� $��30��6   $��30��6    
������  � ToolTip$ Import Data��5/   3   ��6   �� *ImportDataCommand+.��50      �� $-��'pack://application:,,,/Icons/Import.png��6   ��51      `� 52      	 `� 	Converter#	 [ 6#   .1�53      �� 
 �� Path$
 SelectedDevice.DeviceId�6"   �� 54      $
 IsPlaybackMode�6"   55      57      (  "System.Windows.Controls.DatePicker  5:      -   5          Name$ StartDatePicker��5:      $	��120��62   $��5q�6>      SelectedDate 5;      �� *PlaybackStartDate+ �� Mode$ TwoWay=�"  Xceed.Wpf.Toolkit.TimePicker  5=      -   5          Name$ StartTimePicker��5=      $��80��67   $��5q�6B   &   Xceed.Wpf.Toolkit.DateTimeUpDown   Format$ 	ShortTime=�6M   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.DateTime, System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]], System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]]   Value 5>      �� *PlaybackStartTime+$ TwoWay=�  5@      -   5        $ EndDatePicker��5@      $	��120��60   $��5q�6<    5A      �� *PlaybackEndDate+$ TwoWay=�  5C      -   5        $ EndTimePicker��5C      $��80��65   $��5q�6@   $ 	ShortTime=�6K    5D      �� *PlaybackEndTime+$ TwoWay=��� 5G      $
��Play��6     SenseButton#��[ 6$   $��60��6I   $��30��6T   ��5H      �� *PlayDataCommand+��5I      `� 5J      #	 [ 6#   .1�5K      ��  �� ElementName$ StartDatePicker��6"   $
 SelectedDate�6@   �� 5L      $ StartTimePicker��6"   $
 Value�6@   �� 5M      $ EndDatePicker��6"   $
 SelectedDate�6>   �� 5N      $ EndTimePicker��6"   $
 Value�6>   5O      5Q      �� 5T      $��Export��6   #��[ 6&   $��60��6K   $��30��6V   $ Export Data��5U   3   ��6   �� *ExportDataCommand+��5V      `� 5W      #	 [ 6#   .1�5X      �� $ StartDatePicker��6"   $
 SelectedDate�6@   �� 5Y      $ StartTimePicker��6"   $
 Value�6@   �� 5Z      $ EndDatePicker��6"   $
 SelectedDate�6>   �� 5[      $ EndTimePicker��6"   $
 Value�6>   �� 5\      $
 SelectedDevice�6"   5]      5_      5`      �� 5c   
   $��2��6   $��0��6   
�������$��1q�6G    �� CornerRadius$ 4��6[   ._�5d      �� -   5         ��Name$ GraphCanvas��5d      ���#D9E8F0$��5,9,0,9q�6?   $	��574��6P   $
��Left=�6\   5i      �� 5j   
   $��3��6   $��0��6"   $ 
Horizontal=�63   $��Center=�6L   .+�5k      �� -   5         ��Name$ ScrollLeftButton��5k        SenseButtonplot#��[ 60   $��◀ Scroll Left��6Y   $��5q�6r   $	��150��6}   $	��100��5l   	   ��5k   �   �� *ScrollLeftCommand+5l   	   �� 5n      -   5        $ ScrollRightButton��5n      #��[ 62   $��Scroll Right ▶��6\   $	��100��5o   	   ��5n   u   �� *ScrollRightCommand+5o   	   �� 5p      -	   5        $ ResumeLiveData��5p      #��[ 6/   $��Resume Live Data��6Y   $��5q�6u   $	��150��6�   ��6�   �� *ResumeLiveCommand+��5q   	   �� *
IsLiveMode+�� 5r      $ Vertical=�6   $��Right=�60   $��Center=�6L   $��10 10q�6g   .+�5t      �� -
   5         ��Name$ 
ZoomSlider��5t       � Minimum$ 1Q�5u       � Maximum$ 10Q�6    �� TickFrequency$ 1Q�5w       �� IsSnapToTickEnabled$
 True��6    $	��100��6<    � Value 5v      �� *	ZoomLevel+$ TwoWay=�5w   <   5x      5|      �� 5~   
   $ 
Horizontal=�6   $��3��6.   $��0��6<   $��0,10,0,0q�6L   $��Center=�6^   .+�5      �� -   5         ��Name$ MinText��5      $
��Bold"�6)   $��14(�6;   �� 5�      -   5        $ MaxText��5�      $
��Bold"�6)   $��14(�6;   �� 5�      -   5        $ AvgText��5�      $
��Bold"�6)   $��14(�6;   5�      � 5�   
   $��1��6   $��4��6    5�      �� 5�      $	��46* �6    �� 5�      $	��14* �6    �� 5�      $	��60* �6    �� 5�      $	��60* �6    �� 5�      $	��60* �6    �� 5�      $
��180* �6    �� 5�      $��9* �6     5�      �� 5�      $��1* �6#   �� 5�      $��1* �6#   .+�5�      � $��0��6   $��0��6    $��2��60   $��2��6D    5�      �� 5�      $��1* �6$   �� 5�      $��1* �6$    5�      �� 5�      $��1* �6'   �� 5�      $��1* �6'   .+�5�      �� -   5         ��Name$ lblThreshold��5�      $��0��6.     
SenseLabel#��[ 6<   .��6a   Threshold Setup6r   �� 5�      -   5         ��Name$ ThresholdTextBox��5�      $��60��64   $��10,10,10,10q�6?   $��25��6T   $��1��6`   $��0��6m   $��Center=�6~   $
��Left=�6�   ��6�   �� *ThresHoldValue+ �� UpdateSourceTrigger$ PropertyChanged=��� 5�      $��1��6   $��1��6'   $��Update��68   #��[ 6I   $
��Left=�6o   $��65��6�   $��30��6�   ��6�   �� *UpdateThresholdCommand+��5�      �� *Text+$ ThresholdTextBox��5�      �� 5�      -   5        $ HistoCanvas��5�   W   $��3��6   $��0��6"   $��2��62   $��3��6F   $	��250��6l   $	��100��6y   ���#D9E8F0$��
49,5,49,46q�6�   5�      5�      5�      