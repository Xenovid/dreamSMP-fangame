-> DONE

EXTERNAL levelUp()

VAR attackBonus = 0
VAR defenceBonus = 0
VAR pointsBonus = 0
VAR healthBonus = 0
VAR skillName = ""
VAR characterName = ""
VAR itemName = ""
VAR totalEXP = 0
VAR totalGold = 0

EXTERNAL updateEXPGold()
EXTERNAL playSound(soundName)
EXTERNAL GetNextBattleItem()
EXTERNAL displayPortrait(characterName, feeling)
EXTERNAL setTextSound(soundName)
EXTERNAL playSong(songName)
== tutorial ==
    =textbox1   
        works how you'd think a text box would work. Just remember that you can skip to the end of dialogue by clicking with your mouse.
        Also if your afraid of losing progress you can save at any bell!
        ->DONE
    =techno1
        This button is for a basic attack! Dealing a small amount of damage and inflicting the bleeding status effect
        -> DONE
    =techno2
        This button is for your skills! Use blood to perform special attacks on your opponents
        ->DONE
    =techno3
        This indicates that an enemy is affected by the bleeding status effect. Every so often they will take bleeding damage accorrding to how many times they've been afflitcted with the bleeding status effect. All damage done from bleeding is put into Technoblade blood meter, allowing him to perform his skills.
        ->DONE
    =techno4
        This button is for items! Using a item will typically end up healing Technoblade.
        ->DONE
    =techno5
        This button is for running! However I haven't yet implemented it... But once it is it'll probably be used to run from insignificant battles
        ->DONE
    =techno6
        Technoblade doesn't die once his hp drops to 0. Instead the blood meter will used for his health instead. He will then only die once his blood reaches 0. 
        ->DONE
== aintro ==
    =tree
    it's just a tree
    -> END
== devdoor ==
    Sadly this is the current end of this story
    Quite pitful isn't it...
    What's is inside the pyramid
    Will Wilbursoot form a cult that last more than a one episode
    And will Technoblade get his cookies
    I hope you enjoyed what little I had to show you and that you will one day come back to play a more finished game
    -> END
== thebell ==
    = thefirstbell
        ~playSound("sellouttimer")
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        #unskipable
        #slow
        . . .
        ~setTextSound("bell")
        #technoblade
        subscribe to Technoblade!!!!!
        -> DONE
== story ==
    = intro
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        Staaarting the streeeam
        technolate?
        Look, you can't blame me. I was inches from hitting the go live button, then my  door flung open. The super volcano underneath Yellow Stone Park is about to erupt. You gotta believe me.
        Luckily, the stuation is sorted. No ancient volcano is going to stop Technoblade from streaming Minecraft.
        Anyways, the note I got last time said to go here for some free fresh cookies.
        It's a trap?
        Look it could be a trap and I'm ganged up on by 5 people and I lose one of my three lives. Or I could get cookies. I think the benefits vastly outweigh the potential downsides.
        Well Eret's pyramid is just around the corner, I just need to follow the path.
        -> DONE
    =wilbur
        #playable
        WilburBattle
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        Wilbur?
        ~displayPortrait("Wilbur", "surprised")
        ~setTextSound("Wilbur")
        oh hey, it's THE blade
        ~displayPortrait("Wilbur", "default")
        are you here to worship the
        great
        almighty
        Penultimate
        Pyramid
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        No, I heard there's free cookies
        ~displayPortrait("Wilbur", "kindaangry")
        ~setTextSound("Wilbur")
        free cookies???
        technoblade, this structure is so much more then just a place where you can get cookies.
        Can't you understand, its THE 
        great
        almighty
        Penultimate
        Holy
        Pyramid
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        Wilbur, have you even been in the pyramid?
        ~displayPortrait("Wilbur", "default")
        ~setTextSound("Wilbur")
        Me? No I haven't
        But that's not important. The moment I saw it 20 minutes ago, I knew it was special.
        One day may it open its magnificent locked doors for all to trensend into the next world
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        Hmm, I can't believe I didn't notice this before
        ~displayPortrait("Wilbur", "choatic")
        ~setTextSound("Wilbur")
        YEESSSSS, TECHNOBLADE. Now can you truly see the greater meaning of the Pyramid.
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        No Wilbur it isn't that
        I found a key that says its to open up the pyramid
        ~displayPortrait("Wilbur", "silent")
        ~setTextSound("Wilbur")
        ...
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        What, did I say something wrong?
        ~displayPortrait("Wilbur", "silent")
        ~setTextSound("Wilbur")
        ~playSong("WilburTheme")
        Technoblade, I'm going to need those keys
        #battle
        Wilbur
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        If you want to go so bad, you could just follow me in
        ~displayPortrait("Wilbur", "disgusted")
        ~setTextSound("Wilbur")
        Its not that man. The problem is that my savior is a pig who has quite literally gone on the record on being an atheist. It just doesn't sit right
        ~displayPortrait("Technoblade", "default")
        ~setTextSound("technoblade")
        would a cookies help?
        ~displayPortrait("Wilbur", "disgusted")
        ~setTextSound("Wilbur")
        you've ruined my appetite, JUST GO!
        -> DONE
        
=== function updateLevelInfo(name, SkillName, AttackBonus, DefenceBonus, PointBonus, HealthBonus)
     ~ characterName = name
     ~ healthBonus = HealthBonus
     ~ skillName = SkillName
     ~ attackBonus = AttackBonus
     ~ defenceBonus= DefenceBonus
     ~ pointsBonus = PointBonus
=== function updateItemInfo(ItemName)
    ~ itemName = ItemName
=== function updateEXPandGold(exp, gold)
    ~ totalEXP = exp
    ~ totalGold = gold
== victory ==
~updateEXPGold()
You Won
Technoblade gained {totalEXP} EXP and {totalGold} gold
-> items
    = items
        ~GetNextBattleItem()
        {itemName != "": Obtained {itemName}| -> levelupdetails}
        -> items
    = levelupdetails
        ~levelUp()
        {characterName == "":  ->DONE}
        {characterName} leveled up!
        {characterName} gained {attackBonus} attack, {defenceBonus} defence, {pointsBonus} blood, and {healthBonus} hp
        {skillName != "": {characterName} unlocked {skillName}| ->levelupdetails}
        ->levelupdetails
        
    

    
























