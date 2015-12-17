' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
'Necessary library imports for shapes, colors, timers, multiple timers
Imports Windows.UI.Xaml.Shapes
Imports Windows.UI.Colors
Imports System.Threading
Imports Windows.UI.Input

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    'class variables needed in multiple areas of the program
    Dim bubbles() As Ellipse 'bubbles array
    Dim bubble As Ellipse
    Dim colors() As Brush = {New SolidColorBrush(Blue), New SolidColorBrush(Orange), New SolidColorBrush(SeaGreen), New SolidColorBrush(Fuchsia), New SolidColorBrush(LightBlue),
                           New SolidColorBrush(Red), New SolidColorBrush(Purple), New SolidColorBrush(Yellow), New SolidColorBrush(BlueViolet), New SolidColorBrush(Magenta)} 'colors array
    Dim arraySize As Integer = 10 'array size so can resize if needed
    Dim speed() As Integer = {-4, -16, -12, -8, -4, 4, 8, 12, 16, 4} 'level 1 speed array
    Dim speedLevel2() As Integer = {-30, -25, -20, -15, -10, 10, 15, 25, 30, 35} 'level 2 speed array
    Dim moveDown() As Boolean = {True, True, False, False, False, True, True, False, True, True} 'up/down direction array
    Dim moveRight() As Boolean = {False, False, True, True, True, False, False, True, False, False} 'left/right direction array
    Dim i As Integer = 0 'counter integer
    Dim generator As Random = New Random 'random number generator
    Dim bubbleTimer As DispatcherTimer = New DispatcherTimer() 'timers 
    Dim gameTimer As DispatcherTimer = New DispatcherTimer()
    Dim level2Timer As DispatcherTimer = New DispatcherTimer()
    Dim score As Integer 'score per bubble pop
    Dim totalScore As Integer = 0 'total score for one round of game
    Dim bestScore As Integer = 0 'best score during play of all rounds
    Dim pointerOn As Boolean = True 'boolean for pointer press event

    'bubble array is created and loaded at grid load
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)

        ReDim bubbles(arraySize)  'redim bubbles array in case program wants to change amount of bubbles in game

        ' adding bubbles to the canvas of colors in colors array
        For i As Integer = 0 To arraySize - 1
            bubble = New Ellipse
            bubble.Height = 80
            bubble.Width = 80
            bubble.Fill = colors(i)
            bubble.Margin = New Thickness(generator.Next(975), generator.Next(650), 0, 0)
            bubbles(i) = bubble
            myCanvas.Children.Add(bubbles(i))
        Next

        ' setting up the timer for level 1

        AddHandler bubbleTimer.Tick, AddressOf randMoveBubbles
        bubbleTimer.Interval = New TimeSpan(0, 0, 0, 0, 25)
        bubbleTimer.Start()

        'timer for game stop at 10 seconds
        AddHandler gameTimer.Tick, AddressOf endGame
        gameTimer.Interval = New TimeSpan(0, 0, 0, 10)
        gameTimer.Start()
        'timer for level 2
        AddHandler level2Timer.Tick, AddressOf levelTwo
        level2Timer.Interval = New TimeSpan(0, 0, 0, 0, 25)
        level2Timer.Stop()

    End Sub
    'sub that bubbletimer for level 1 handles.
    Sub randMoveBubbles()

        For i As Integer = 0 To 9 'iteration through array
            'speed and updown direction movement of bubbles
            If moveDown(i) Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                bubbles(i).Margin = New Thickness(currLeftM + speed(i), currTopM + speed(i), 0, 0)
            End If
            'conditional to handle when bubble goes off the canvas-resets margins to a random location back on the canvas
            If bubbles(i).Margin.Left > 975 Or bubbles(i).Margin.Top > 650 Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                Dim left As Integer = generator.Next(0, 890)
                Dim top As Integer = generator.Next(0, 570)
                bubbles(i).Margin = New Thickness(left + speed(i), top + speed(i), 0, 0)
                Dim newColorIndex As Integer = generator.Next(colors.GetUpperBound(0) - 1)
                bubbles(i).Fill = colors(newColorIndex)
            End If
            'conditional to handle when the bubble is popped. relocates bubble to a new random margin on the canvas
            If bubbles(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                Dim left As Integer = generator.Next(0, 890)
                Dim top As Integer = generator.Next(0, 570)
                bubbles(i).Margin = New Thickness(left + speed(i), top + speed(i), 0, 0)
                Dim newColorIndex As Integer = generator.Next(colors.GetUpperBound(0) - 1)
                bubbles(i).Fill = colors(newColorIndex)
                bubbles(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            End If
            'conditional to handle speed and left and right movement of bubbles
            If moveRight(i) Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                bubbles(i).Margin = New Thickness(currLeftM + speed(i), currTopM - speed(i), 0, 0)
            End If
        Next
    End Sub
    'event to handle when the pointer is pressed on the form
    Private Sub Grid_PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        If pointerOn = True Then 'boolean to designate when in the program to allow function of pointerPressed event

            For i As Integer = 0 To bubbles.GetUpperBound(0) - 1
                ' declaring the pointer and x and y position of pointer
                Dim pointer As PointerPoint = e.GetCurrentPoint(myCanvas)
                Dim x As Integer = CInt(pointer.Position.X)
                Dim y As Integer = CInt(pointer.Position.Y)
                'setting pointer to know when to collapse the visibility of the bubble (popping the bubble)
                If x >= bubbles(i).Margin.Left And x <= bubbles(i).Margin.Left + bubbles(i).Width _
                    And y >= bubbles(i).Margin.Top And y <= bubbles(i).Margin.Top + bubbles(i).Height Then
                    bubbles(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed

                    'conditions to allow for various points for different colors and negative points for red bubble
                    If bubbles(i).Fill Is colors(5) Then
                        score = -2
                    ElseIf bubbles(i).Fill Is colors(1) Then
                        score = 3
                    ElseIf bubbles(i).Fill Is colors(2) Then
                        score = 4
                    ElseIf bubbles(i).Fill Is colors(3) Then
                        score = 5
                    ElseIf bubbles(i).Fill Is colors(4) Then
                        score = 0
                    ElseIf bubbles(i).Fill Is colors(6) Then
                        score = -1
                    ElseIf bubbles(i).Fill Is colors(7) Then
                        score = 7
                    ElseIf bubbles(i).Fill Is colors(8) Then
                        score = 8
                    Else
                        score = 10

                    End If
                    totalScore = totalScore + score 'keeping tally of the total score of bubbles popped
                    keepScore.Text = totalScore 'putting score in the keep score text block
                End If
            Next
        End If

    End Sub

    'sub to be handled by the gameTimer addressOf
    Sub endGame()
        bubbleTimer.Stop() 'stops the bubble animation
        level2Timer.Stop() 'stops the level2 animation
        pointerOn = False  'stops the pointer pressed event from being able to be used
        If totalScore > bestScore Then  ' resets the scores and records best score if current game is higher than previous best score
            bestScore = totalScore
        End If
        bestKeepScore.Text = bestScore 'records the best score on the form in best score keeper text block
        gameTimer.Stop() 'stops the gameTimer

    End Sub
    'quit button to exit the game
    Private Sub Quit_Click(sender As Object, e As RoutedEventArgs) Handles Quit.Click
        App.Current.Exit()
    End Sub
    'play again button to take player to level 2 and start the game again
    Private Sub playAgain_Click(sender As Object, e As RoutedEventArgs) Handles playAgain.Click
        level2Timer.Start() 'starts level 2 timer
        pointerOn = True  'restarts the pointer pressed event
        gameTimer.Start()  'restarts the game timer
        keepScore.Text = 0  'resets score text block on the form to 0
        totalScore = 0   'resets the total score variable
        levelNumber.Text = 2  'sets the level to level 2 on the textblock on the form

    End Sub
    'sub that is handled by the level2Timer - everything here is exact same as randMoveBubbles above
    'EXCEPT - the speed is increased using a speedLevel2 array
    Sub levelTwo()

        For i As Integer = 0 To 9
            'using speed level 2 array for faster moving animation
            If moveDown(i) Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                bubbles(i).Margin = New Thickness(currLeftM + speedLevel2(i), currTopM + speedLevel2(i), 0, 0)
            End If
            'using speed level 2 array for faster moving animation
            If bubbles(i).Margin.Left > 975 Or bubbles(i).Margin.Top > 650 Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                Dim left As Integer = generator.Next(0, 890)
                Dim top As Integer = generator.Next(0, 570)
                bubbles(i).Margin = New Thickness(left + speedLevel2(i), top + speedLevel2(i), 0, 0)
                Dim newColorIndex As Integer = generator.Next(colors.GetUpperBound(0) - 1)
                bubbles(i).Fill = colors(newColorIndex)
            End If
            'using speed level 2 array for faster moving animation
            If bubbles(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                Dim left As Integer = generator.Next(0, 890)
                Dim top As Integer = generator.Next(0, 570)
                bubbles(i).Margin = New Thickness(left + speedLevel2(i), top + speedLevel2(i), 0, 0)
                Dim newColorIndex As Integer = generator.Next(colors.GetUpperBound(0) - 1)
                bubbles(i).Fill = colors(newColorIndex)
                bubbles(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            End If
            'using speed level 2 array for faster moving animation
            If moveRight(i) Then
                Dim currLeftM As Integer = bubbles(i).Margin.Left
                Dim currTopM As Integer = bubbles(i).Margin.Top
                bubbles(i).Margin = New Thickness(currLeftM + speedLevel2(i), currTopM - speedLevel2(i), 0, 0)
            End If
        Next
    End Sub
End Class

