# CastingWaver

A funny thing inspired by HexCasting.

> It will be very easy to get started if you've ever played this mod

Cast some spells by drawing lines in hex grid.

---

### How to draw

Click MouseLeft and drag.

The first line will always be ignored.

Relative to previous line,there is 5 direction you can move(besides move back),

- move front : 'W'

- move front_left : 'Q'

- move back_left : 'A'

- move front_right : 'E'

- move back_right : 'D'

##### Refer to keyboard:

QWE

A   D

---

### Guide for developer

use HexPattern.AddPattern(string pattern, Action spell) to register your custom pattern.

use SpellStackManager.PushStack(Variant variant) to push variant in stack.

use SpellStackManager.PopStack() to pop and get variant on top of stack.
