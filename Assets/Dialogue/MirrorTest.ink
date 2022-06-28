INCLUDE globals.ink

{mirror == false: -> main | -> already_chose }

=== main ===

Hello

I am bored

Do you want to play a game ?

*[Yes]

    Good !
    
    ~mirror = true
    
    -> END
    
    *[No]
    
    You are boring !
    
    ~mirror = false
    
    -> END
    
    === already_chose ===
    
    Well done.
    
    -> END