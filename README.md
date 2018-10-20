# UModule 2D Camera

## Contents
- 2D perspective camera extensions
- Camera motion behaviors
- Camera blocking zones

## Dependencies
- UM_Basic `https://github.com/rajin-s/UM_Basic.git`

## Setup
- Navigate to the desired install location inside a Unity project git repository
  ```
  cd Assets/Modules
  ```
- Inside the target folder, add this repository as a submodule
  ```
  git submodule add https://github.com/rajin-s/UM_Camera2D.git
  ```
- Commit the changes to finish registering the submodule
  ```
  git add --all; git commit -m "Added UM_Camera2D submodule"
  ```
- Initialize the submodule from anywhere in the parent repository
  ```
  git submodule init
  ```

## Updating
_requires setup as a git submodule according to the above instructions_
- Inside the parent Unity project repository, enter
  `git submodule update --remote`
  to get the latest version of this submodule

## License
Available according to the terms in `UM_Camera2D/LICENSE` (GNU GPL3)