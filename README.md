https://omerzshahar.itch.io/one-patient-left


- for UML:
- [Download the UML](Assets/Planning/One-Patient-Left UML.pdf](https://github.com/Game-development-project-LLC/One-Patient-Left/blob/main/Assets/Planning/One-Patient-Left-UML.pdf)

# One Patient Left 

## תיאור קצר של המשחק
משחק התגנבות קצר בבית חולים:
השחקן מתעורר בחדר, צריך לאסוף חפצים (כרטיס מגנטי, תמונה, רמזים), להיזהר מזומבים שמסתובבים במסדרון, ולהגיע לדלת היציאה.  
במהלך המשחק מופיעות הוראות, טקסטים וחלון Game Over.

---

## מבנה כללי של הסקריפטים

### 1. Interactable2D
מחלקת בסיס לכל אובייקט בעולם שהשחקן יכול לתקשר איתו (מיטה, חלון, רמז, דלת וכו').

**פונקציות חשובות**
- `PromptText` – תכונה שמחזירה את טקסט ההנחיה שמופיע ליד השחקן
- `Interact(PlayerInteraction2D player)` – פונקציה אבסטרקטית; כל מחלקת ירושה מממשת מה קורה בפועל בזמן אינטראקציה.

---

### 2. PlayerInteraction2D
אחראי לזהות אובייקטים מסוג Interactable2D שהשחקן עומד לידם ולהפעיל את האינטראקציה בלחיצה על מקש.

**פונקציות חשובות**
- `OnTriggerEnter2D(Collider2D other)` – בודק אם נכנסנו לאזור של אובייקט אינטראקטיבי, שומר אותו כיעד נוכחי ומציג טקסט הנחיה דרך UIManager.
- `OnTriggerExit2D(Collider2D other)` – מנקה את היעד הנוכחי ומסתיר את טקסט ההנחיה כשמתרחקים.
- `Update()` – מאזין ללחיצה על מקש האינטראקציה (למשל E) ואם יש יעד נוכחי, קורא ל־`Interact()` עליו.

---

### 3. PlayerMovement2D
שולט על תנועת השחקן במפה (תנועת טופ־דאון) ומאפשר לעבור בין מהירות רגילה למהירות איטית.

**שדות חשובים**
- מהירויות – `normalSpeed`, `slowSpeed`.
- צירי קלט – `horizontalAxisName`, `verticalAxisName`.
- מקש מעבר למהירות איטית – `slowToggleKey`.

**פונקציות חשובות**
- `Update()` – קוראת קלט מהמקלדת ומטפלת בהחלפת מהירות (רגילה / איטית).
- `FixedUpdate()` – מזיז את השחקן באמצעות Rigidbody2D.
- `ReadMovementInput()` – מחשבת וקטור כיוון מתנועת WASD/חצים.
- `HandleSpeedToggle()` – הופך דגל שמשתמש במהירות איטית / רגילה.
- `MoveCharacter()` – מבצע את ההזזה הפיזית.
- `getNormalSpeed()` / `getSlowSpeed()` – מחזירות את מהירויות הבסיס (משמש, למשל, את הזומבים).
- `CurrentSpeed`, `IsUsingSlowSpeed` – תכונות מידע על מצב המהירות הנוכחי.

---

### 4. PlayerInventory
אחראי על “תיק החפצים” של השחקן – רשימת מחרוזות שמייצגות פריטים (כרטיס, תמונה, רמזים).

**פונקציות חשובות**
- `AddItem(string itemId)` – מוסיף פריט למלאי אם הוא לא קיים עדיין.
- `HasItem(string itemId)` – בודק אם פריט מסוים נמצא במלאי (למשל "staff_keycard").
- `GetInventoryText()` – מחזיר טקסט קריא של כל הפריטים (לצורך תצוגה לשחקן).
- `Update()` – מאזין למקש (למשל I) ומציג/מסתיר את תוכן המלאי דרך UIManager.

---

### 5. UIManager
מנהל את כל ה־UI הבסיסי במשחק: טקסט הנחיה, טקסט מידע, ומסך Game Over.  
ממומש כסינגלטון (Instance) כך שכל סקריפט אחר יכול לקרוא לו בקלות.

**פונקציות חשובות**
- `ShowPrompt(string text)` / `HidePrompt()` – מציגים או מסתירים טקסט קצר ליד השחקן (הנחיית אינטראקציה).
- `ShowInfo(string text)` / `ClearInfo()` – מציגים או מנקים טקסט מידע על המסך (תיאור חפצים, רמזים וכו').
- `ShowGameOver(string message)` / `HideGameOver()` – מפעילים או מסתירים את מסך ה־Game Over והטקסט שבו.
- `RestartLevel()` – טוען מחדש את הסצנה הנוכחית (מחובר לכפתור Restart במסך Game Over).

---

### 6. SimpleInteractable2D
מימוש גנרי למחלקת בסיס Interactable2D – משמש לרוב האובייקטים הפשוטים בחדר: מיטה, חלון, רמזים, חפצים.

**סוגי אינטראקציה (enum SimpleInteractionType)**
- `ShowText` – רק מציג טקסט (למשל חלון, מיטה).
- `PickupItem` – מוסיף פריט ל־Inventory (כרטיס, תמונה, רמז).
- `RemoveObject` – מעלים/מכבה אובייקט מהעולם (למשל קיר שחור שנפתח).

**שדות חשובים**
- `interactionType` – איזה סוג אינטראקציה האובייקט הזה מבצע.
- `infoText` – הטקסט שיופיע דרך UIManager.
- `itemId` – מזהה הפריט שיתווסף למלאי (אם מדובר ב־PickupItem).
- `destroyAfterInteract` – האם להרוס את האובייקט עצמו אחרי אינטראקציה.
- `canInteractMultipleTimes` – האם ניתן לחזור על האינטראקציה יותר מפעם אחת.
- `objectToRemove`, `deactivateInsteadOfDestroy` – הגדרות עבור מצב RemoveObject.

**פונקציות חשובות**
- `Interact(PlayerInteraction2D player)` – הנקודה המרכזית: מחליטה האם לבצע אינטראקציה (בודקת אם כבר השתמשנו, אם אפשר לחזור וכו') וקוראת ל־`HandleInteraction`.
- `HandleInteraction(PlayerInteraction2D player)` – מפעילה את הפעולה המתאימה לפי `interactionType`.
- `ShowInfoText()` – מציגה את `infoText` דרך UIManager.
- `PickupItem(PlayerInteraction2D player)` – מוסיפה פריט ל־PlayerInventory ומציגה טקסט.
- `RemoveObject()` – מכבה או הורס אובייקט (קיר, מחסום וכו') ומציג טקסט.

---

### 7. ExitDoorInteractable
מחלקת אינטראקציה ספציפית לדלת היציאה מהחדר. יורשת מ־Interactable2D.

**שדות חשובים**
- `lockedMessage` – הטקסט שמופיע כשאין לשחקן את הפריט הדרוש (הדלת נעולה).
- `openMessage` – טקסט שמופיע כשהדלת נפתחת.
- `requiredItemId` – המזהה של הפריט הנדרש (למשל "staff_keycard").
- `nextSceneName` – שם הסצנה הבאה (המסך "Next level coming soon" וכו').

**פונקציות חשובות**
- `Awake()` – מוודאת שלדלת יש Collider2D במצב Trigger.
- `Interact(PlayerInteraction2D player)` – בודקת אם לשחקן יש את הפריט הדרוש; אם כן מציגה הודעת פתיחה ומעבירה לסצנה הבאה, אם לא – מציגה הודעת “נעול”.

---

### 8. InstructionsController
אחראי על מסך ההוראות בתחילת המשחק ועל האפשרות לפתוח/לסגור אותו בזמן המשחק.

**שדות חשובים**
- `instructionsPanel` – ה־Panel ב־Canvas שמכיל את טקסט ההוראות.
- `playerMovement` – רפרנס ל־PlayerMovement2D כדי לעצור תנועה בזמן ההוראות.
- `showOnStart` – האם לפתוח את מסך ההוראות אוטומטית בתחילת הסצנה.
- `toggleKey` – המקש שמאפשר לפתוח/לסגור הוראות בזמן המשחק (למשל H).

**פונקציות חשובות**
- `Start()` – אם מוגדר, מציג את ההוראות בתחילת השלב.
- `Update()` – מאזין למקש ומחליט האם לקרוא ל־`ShowInstructions` או `HideInstructions`.
- `ShowInstructions()` – מפעיל את ה־Panel ועוצר את תנועת השחקן.
- `HideInstructions()` – מסתיר את ה־Panel ומחזיר לשחקן את השליטה בתנועה.

---

### 9. ZombieChasePlayer2D
התנהגות “זומבי רודף”: הזומבי עומד במקום עד שהשחקן מתקרב לרדיוס מסוים ואז מתחיל לרדוף אחריו.

**שדות חשובים**
- `moveSpeed` – מהירות התנועה של הזומבי.
- `matchPlayerSpeed` – האם לשאוב את מהירות השחקן בתחילת המשחק (כדי שיזוז בערך באותה המהירות).
- `detectionRadius` – מרחק שבו הזומבי מתחיל לרדוף.
- `loseRadius` – מרחק שבו הזומבי מפסיק לרדוף.
- `playerTransform` – רפרנס למיקום השחקן (מוגדר אוטומטית אם לא הוגדר ידנית).

**פונקציות חשובות**
- `Awake()` – מחפש את PlayerMovement2D ואם צריך מתאם את מהירות הזומבי למהירות השחקן.
- `FixedUpdate()` – בכל פריים:
  - מחשב מרחק מהשחקן.
  - מחליט אם להתחיל רדיפה / לעצור רדיפה.
  - אם במצב רדיפה – מזיז את הזומבי לכיוון השחקן.
- `OnDrawGizmosSelected()` – מצייר עיגולים ב־Scene View שמייצגים את רדיוס הגילוי ורדיוס האיבוד (כלי עזר למפתח).

---

### 10. ZombieKillPlayer2D
התנהגות פשוטה לזומבי שהורג את השחקן במגע ומציג מסך Game Over.

**שדות חשובים**
- `gameOverMessage` – הטקסט שיופיע כששחקן נתפס.

**פונקציות חשובות**
- `Awake()` – מוודאת שה־Collider2D של הזומבי **אינו** Trigger, כדי שלא יעבור דרך קירות.
- `OnCollisionEnter2D(Collision2D collision)` – אם האובייקט שהתנגש הוא השחקן:
  - מכבה לו את ה־PlayerMovement2D (שלא יוכל לזוז).
  - מציג את מסך ה־Game Over דרך UIManager עם ההודעה המתאימה.

---

## סיכום
הקוד מחולק למודולים ברורים:

- **Player** – תנועה, אינטראקציה ומלאי.
- **World / Interactables** – חפצים בחדר, דלתות, רמזים וקירות מיוחדים.
- **UI** – טקסטים, מסך הוראות ומסך Game Over.
- **Enemies** – זומבים שרודפים אחרי השחקן ותופסים אותו.

החלוקה הזו מאפשרת להוסיף בקלות עוד סוגי חפצים, זומבים, שלבים ומסכי UI בלי לשבור את שאר הקוד.


## Team
- Omer (omerzshahar@gmail.com)
- Matan (Matanbrimer1@gmail.com) 
