   M S B A M L   `   `   `  ����I  ENETMQ_Tutorial, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:NETMQ_TutorialNETMQ_Tutorial  5   	       NETMQ_Tutorial.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:NETMQ_Tutorial     5   	   -   5          � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5
      �  � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� $	��IP:��6   $
��Left=�6"   $��Center=�6=   $��15(�5      
���   �$
��Bold"�65   �� 5      -   5         ��Name$ Txt_IP��5      $��	10,0,10,0q�5      $
��Bold"�6)   $��15(�6;   $��192.168.1.56��6I   
���   �$��Center=�5   ;   $��Center=�5      �� 5      -   5         ��Name$ Btn_Connect��5      $��Connect��6*   $��Center=�6<   $��Center=�5      $��15(�6J   $
��Bold"�5      �� 5      -   5        $ 	Btn_Share��5      $��Share��6(   $��Center=�68   $��Center=�5      $��15(�6H   $
��Bold"�5      $��	10,0,10,0q�6'   5      �� 5   
   -   5         ��Name$ ImageBox��5      $��Uniform=�6"   $��1��64   5      5      