   M S B A M L   `   `   `  ����N  JChatClientFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:ChatClientFrameworkChatClientFramework  5   	   4    .GrpcWpfSample.Client.Wpf.View.ChatClientWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   3local!clr-namespace:ChatClientFramework     5   	     � Title$  Chat Client��5   	   $��459.6��5	   	   $��754.4��5
   	   .��5      � ��5   
   �� 5       �� ImageSource$ background2.jpg��6    j� 
AlignmentX$
 Left=�67    j� Stretch$ Uniform=�6I    � RowDefinitions 5   
   �� 5      .+�5   
   �� -   5         ��Name$ History��5      ���	#FFA65C33$��0��5   &   $��235,0,0,43.6q�63   ���	#FFFFD600$��14(�6`   $��Segoe Print)�6n   ��5      �� *ChatHistory+��5      �� 5      .R�5      � .+�5      �� $��2q�6$   $��0��5   m   ��5   /   ��  �� Path$ Item1���6K   �� $ Item2���5   $   �� $ Item3���6K   �� $ Item4�6m   5      5      5      �� 5!   
   -   5         ��Name$ 	NameInput��5!       �� TextAlignment$ Center=�6%   $��235,377,332,0.6q�5"   ,   $��Segoe Print)�6E   $��15(�6^   ���	#FFFFD600���	#FFFAFAFA$
��Bold"�6�   ��6   �� *Name+6�   �� 5#   
   -   5        $ 	BodyInput��5#      $��406,377,0,-0.4q�5%   0   ���	#FFA65C33$��Segoe Print)�6_   
�������$
��Bold"�6�   �� 5'   
   -   5        	 ��Name$	 Users��5'      $��0��5(      $��0q�5)      
������ $��0,38,513,43.6q�5+   /   ���	#FFFDFAFA$��Segoe Print)�6]   $��18(�6v   $
��Bold"�6�   ��6   �� *	UsersList+
 J� Effect
 5,      3  -System.Windows.Media.Effects.DropShadowEffect  5-         Color$ 	#FFA65C33��6#   5/      �� 50   
   -   5        $
��join��51   '   $
��Join��50      $
��Left=�6A   $��0,377,0,-0.4q�51      $	��235��52      ���	#FFA65C33���	#FFFFD600$��Segoe Print)�53   (   $
��Bold"�54      $��20(�6#   ��50       �� *ClickCommand+54   1   �� 56   
    �� TextAlignment$ Center=�6   $
��Left=�6,   $��10,0,0,0q�6G   $
��Wrap=�6Y   $��Users��6m   $	��Top=�57      $	��225��6-   $��38��69   $$��Gill Sans Nova Cond Ultra Bold)�58      $��24(�6A   $
��Bold"�6O   ���	#FFA65C3359      5:      5;      