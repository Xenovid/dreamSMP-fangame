using Unity.Entities;
using UnityEngine.UIElements;
using Unity.Scenes;
using UnityEngine;

public class OptionMenuSystem : SystemBase
{
      private Slider volume;
      private optionMenuSelectables currentSelection = optionMenuSelectables.volume;
      private SceneSystem sceneSystem;
      private float audioVolume;
      private bool isVolumeSet = false;

      private Entity titleSubScene;
      private Entity optionsSubScene;

      protected override void OnStartRunning()
      {
            audioVolume = AudioManager.volume;
            sceneSystem = World.GetOrCreateSystem<SceneSystem>();
            isVolumeSet = false;

            Entities
            .WithoutBurst()
            .WithAll<OptionsSubSceneTag>()
            .ForEach((Entity ent) => {
                  optionsSubScene = ent;
            }).Run();
            Entities
            .WithoutBurst()
            .WithAll<TitleSubSceneTag>()
            .ForEach((Entity ent) => {
                  titleSubScene = ent;
            }).Run();
      }

      protected override void OnUpdate()
      {
            EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
            UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            Entities
            .WithStructuralChanges()
            .WithoutBurst()
            .WithAll<OptionMenuTag, TitleMenuTag>()
            .ForEach((in UIDocument UIDoc) =>{
                  VisualElement root = UIDoc.rootVisualElement;
                  if(root == null){
                        Debug.Log("root not found");
                  }
                  else{
                        Slider volumeSlider = root.Q<Slider>("volume_slider");
                        if(!isVolumeSet){
                              volumeSlider.value = audioVolume;
                              isVolumeSet = true;
                        }
                        switch(currentSelection){
                              case optionMenuSelectables.back:
                                    if(input.goselected || input.goback){
                                          sceneSystem.UnloadScene(optionsSubScene);
                                          sceneSystem.LoadSceneAsync(titleSubScene);
                                    }
                                    else if(input.moveup){
                                          currentSelection = optionMenuSelectables.volume;
                                    }
                                    break;
                              case optionMenuSelectables.volume:
                                          if(input.goback){
                                                AudioManager.playSound("menuchange");
                                                sceneSystem.UnloadScene(optionsSubScene);
                                                sceneSystem.LoadSceneAsync(titleSubScene);
                                                isVolumeSet = false;
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
