   M S B A M L   `   `   `  ����D  @PowerNote, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:PowerNote	PowerNote  5      > :CJT, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&clr-namespace:CJT;assembly=CJTCJT 5      !    PowerNote.TreeSettingsPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      )localclr-namespace:PowerNote     5      .CJTclr-namespace:CJT;assembly=CJT     5      $	��400��5	      .+�5
        CJT.PropertyPanel  $	��Top=�6     � DataContext  6-   �� *	NRPanelVM+.+�5   
     CJT.TextBlock  $	��150��6   Show Nodes/Relationships:6?   �� 5   
   -   5         ��Name$ NRPanel��5      $	��150��5      ��5      �� *Items+ �� SelectedItem 5      �� *SelectedItem+ �� Mode$ TwoWay=��� 5   
    d� 	IsChecked 6   �� *	IsApplied+$ TwoWay=�.��6@   Apply6G   5        5      $	��Top=�6     6-   �� *TypePanelVM+.+�5   
     $	��150��6   Filter by type:65   �� 5   
   -   5        $ 	TypePanel��5      $	��150��5      ��5      �� *Items+ 5      �� *SelectedItem+$ TwoWay=��� 5   
    6   �� *	IsApplied+$ TwoWay=�.��6@   Apply6G   5        5      $	��Top=�6     6-   �� *GroupPanelVM+.+�5   
     $	��150��6   Group by properties:6:   �� 5   
   -   5        $ 
GroupPanel��5      $	��150��5      ��5      �� *Items+ 5      �� *SelectedItem+$ TwoWay=��� 5    
    6   �� *	IsApplied+$ TwoWay=�.��6@   Apply6G   5!        5"      $	��Top=�6     6-   �� *StructurePanelVM+.+�5#   
     $	��150��6   Structure by properties:6>   �� 5$   
   -   5        $ StructurePanel��5$      $	��150��5%      ��5&      �� *Items+ 5'      �� *SelectedItem+$ TwoWay=��� 5(   
    6   �� *	IsApplied+$ TwoWay=�.��6@   Apply6G   5)      5*      