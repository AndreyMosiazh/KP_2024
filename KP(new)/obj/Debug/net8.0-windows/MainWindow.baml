   M S B A M L   `   `   `  ����  PharmacyApp    PharmacyApp.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  PharmacyApp��5   	   $	��450��6   $	��800��6*   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��10q�6;   .+�5      �� -   5         ��Name$ SearchTextBox��5      $	��200��6-   $��30��69   5      �� 5      -   5         ��Name$ PlaceholderTextBlock��5      $��	Search...��66    J� IsHitTestVisible$ False��5      
�������$��Center=�5      $��14,0,0,0q�63   5      �� 5   
   -   5         ��Name$ SortComboBox��5      $��1��6)   $��30��66   $��79,10,424,10q�6B   .@�5      �� $��Sort by Price��6   �� 5      $��Sort by Quantity��6   �� 5      $��Sort by Total��6   �� 5      $��Sort by Unit��6   �� 5      $��Sort by Name��6   �� 5      $��Sort by Index��6   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ RecordsDataGrid��5      $��2��6,   $��10q�69      AutoGenerateColumns$ False��6E   ��6a   �� *
_medicines+	   Columns	 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Index��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *Index+  5"      $

 Name��6%    63   �� *Name+  5#      $
 Price��6%    64   �� *Price+  5$      $
 Quantity��6%    67   �� *
Quantity+  5%      $

 Unit��6%    63   �� *Unit+  5&      $
 Total��6%    64   �� *Total+5(      �� 5*   
   $ 
Horizontal=�6   $��1��6.   $��Right=�6;   $��
0,10,10,10q�6W   .+�5+      �� -   5        $��Import��5+      $��75��6&   61   �� 5,      -   5        $��Export��5,      $��75��6&   $��5,0,0,0q�61   6B   �� 5-      -   5        $	��Add��5-      $��75��6#   $��5,0,0,0q�6.   6?   �� 5.      -   5        $
��Edit��5.      $��75��6$   $��5,0,0,0q�6/   6@   �� 5/      -	   5        $��Delete��5/      $��75��6&   $��5,0,0,0q�61   6B   50      51      52      