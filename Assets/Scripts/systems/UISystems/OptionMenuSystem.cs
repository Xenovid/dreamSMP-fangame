using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using UnityEngine;

public class OptionMenuSystem : SystemBase
{
      private Slider volume;
      private optionMenuSelectables currentSelection = optionMenuSelectables.volume;
      private selectionStates selectionState = selectionStates.selecting;
      private SceneSystem sceneSystem;
      private float audioVolume;
      private bool isVolumeSet;

      protected override void OnStartRunning()
      {
            audioVolume = AudioManager.volume;
            sceneSystem = World.GetOrCreateSystem<SceneSystem>();
            isVolumeSet = false;
      }

      protected override void OnUpdate()
      {
            Entities
            .WithStructuralChanges()
            .WithoutBurst()
            .WithAll<OptionMenuTag, TitleMenuTag>()
            .ForEach((UIDocument UIDoc, UIInputData input) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){
                        Debug.Log("root not found");
                  }
                  else{
                        Slider volumeSlider = root.Q<Slider>("volume_slider");
                        if(!isVolumeSet){
                              volumeSlider.value = AudioManager.volume;
                              isVolumeSet = true;
                        }
                        Debug.Log("hello mooo");
                        switch(currentSelection){
                              case optionMenuSelectables.back:
                                    if(input.goselected || input.goback){
                                          sceneSystem.UnloadScene(TitleSubSceneReferences.Instance.OptionSubScene.SceneGUID);
                                          sceneSystem.LoadSceneAsync(TitleSubSceneReferences.Instance.titleSubScene.SceneGUID);
                                    }
                                    else if(input.moveup){
                                          currentSelection = optionMenuSelectables.volume;
                                    }
                                    break;
                              case optionMenuSelectables.volume:
                                          if(input.goback){
                                                AudioManager.playSound("menuchange");
                                                sceneSystem.UnloadScene(TitleSubSceneReferences.Instance.OptionSubScene.SceneGUID);
                                                sceneSystem.LoadSceneAsync(TitleSubSceneReferences.Instance.titleSubScene.SceneGUID);
                                          }
                                          else if(input.moveright){
                                                AudioManager.playSound("menuchange");
                                                if(volumeSlider.value + .1 < volumeSlider.highValue){
                                                      volumeSlider.value = volumeSlider.value + .1f;
                                                }
                                                else{
                                                      volumeSlider.value = volumeSlider.highValue;
                                                }
                                          }
                                          else if(input.moveleft){
                                                AudioManager.playSound("menuchange");
                                                if(volumeSlider.value - .1 > volumeSlider.lowValue){
                                                      volumeSlider.value -= .1f;
                                                }
                                                else{
                                                            volumeSlider.value = volumeSlider.lowValue;
                                                }
                                          }
                              break;
                        }
                        AudioManager.changeVolume(volumeSlider.value);   
                  }        
            }).Run();
      }
}

public enum optionMenuSelectables{
      back,
      volume
}
public enum selectionStates{
      selecting,
      editing
}
