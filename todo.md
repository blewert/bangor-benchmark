#To do list

## 8th November 2015
- Add a panel to the right of the two columns which shows primitive/instance selection for environment
     - Save data for each view, namely primitive/instance selection
     - This will be saved into `bootup.cs` using the callback `onViewUpdate` to update the current place to save view data into, and `onInstancesButtonClick`/`onPrimitivesButtonClick` to get the actual values at the time
     - We can find the primitive/instance in the object array later on by matching the names (button text) to the name in the object array
- Add a panel to the right of the two columns which shows the possible settings for the primitive along with the values in the instance picked
- Check if data has been entered for all three views, if so, then attempt to play
    - Instantiate empty object with script from environment primitive's `scriptToLoad` path with settings from instance
    - This script should take care of the rest of it
    - Do the same for gamemode and character
- Add the *'add instance'* button to the menu
    - Read possible settings of primitive, then read it's type
    - Will need a callback to be hooked from `bootup.cs`
    - Where to handle functionality? `MenuHandler.cs` or `bootup.cs`?
    - This type needs to be matched to a UI component. e.g. `string/choice{...}` needs to be mapped to something like a combobox.
    - This isn't essential and can be left until later


## Timeline
-  **November 15th:** Results for tree survey need to be completed, and writing up of the results and paper needs to be started. `Results nearly finished, writeup not started yet`

- **November 30th:** Dynamic menu needs to be completed. The capture the flag gamemode also needs to be completed. `The menu's basic interaction in terms of UI is almost completed. Stuff needs to be loaded in, and selections need to be saved`

- **December 15th:** Networking client/server stuff needs to be implemented. Possibly create a library for ease of use?

- **December 30th:** The created game needs to be played by people over the holidays.

- **January 30th** to **March 30th**: Write up of dissertation. Includes literature review, tree paper and bangor benchmark stuff.
