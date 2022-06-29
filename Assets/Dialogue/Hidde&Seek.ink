INCLUDE globals.ink


{alreadySaidHi == false: -> Hi | -> main} 

=== Hi ===

Hi... :)

~alreadySaidHi = true

-> main

=== main ===

I want to play hide & seek

I count to {countTimeBeforePlay} & you find a place to hide  

->Question

= Question

What do you say ?

*[Let's do it]

    Good !
    
    I will start now
        
    ~hide = true
    
    -> END
    
    *[What happens if you find me ?]
    
    Mmmmmmm, I might get upset
    
    -> Question
    
    
     *[Not now]
    
    I undestand 
    
    :(
    
    -> END
    
=== alreadyPlayed ===

That was fun

-> END

    
