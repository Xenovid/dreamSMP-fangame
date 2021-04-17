using Unity.Entities;
using Unity.Scenes;
using UnityEngine.UIElements;
using UnityEngine;

public class CreditsMenuSystem : SystemBase
{
    private bool isLinked;
    SceneSystem sceneSystem;

    private Entity creditsSubScene;
    private Entity titleSubScene;

    protected override void OnStartRunning()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
          isLinked = false;

            Entities
            .WithoutBurst()
            .WithAll<TitleSubSceneTag>()
            .ForEach((Entity ent) => {
                titleSubScene = ent;
            }).Run();
            Entities
            .WithoutBurst()
            .WithAll<CreditsSubSceneTag>()
            .ForEach((Entity ent) => {
                creditsSubScene = ent;
            }).Run();
    }
  
    protected override void OnUpdate()
    {
        EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
        UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

        Entities
        .WithStructuralChanges()
        .WithoutBurst()
        .WithAll<CreditsTag>()
        .ForEach((in UIDocument UIDoc) =>{
            VisualElement root = UIDoc.rootVisualElement;
            if(root == null){
                Debug.Log("root not found");
            }
            else{
                Button technoYoutubeButton = root.Q<Button>("techno_youtube");
                Button technoTwitterButton = root.Q<Button>("techno_twitter");

                Button tommyYoutubeButton = root.Q<Button>("tommy_youtube");
                Button tommyTwitterButton = root.Q<Button>("tommy_twitter");
                Button tommyTwitchButton = root.Q<Button>("tommy_twitch");

                Button wilburYoutubeButton = root.Q<Button>("wilbur_youtube");
                Button wilburTwitterButton = root.Q<Button>("wilbur_twitter");
                Button wilburTwitchButton = root.Q<Button>("wilbur_twitch");
                //make it so if the buttons are pressed they send to their according site
                if(!isLinked){
                    technoYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.youtube));
                    technoTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTechno(websites.twitter));

                    tommyYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.youtube));
                    tommyTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitter));
                    tommyTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToTommy(websites.twitch));

                    wilburYoutubeButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.youtube));
                    wilburTwitterButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitter));
                    wilburTwitchButton.RegisterCallback<ClickEvent>(ev => LinkSender.sendToWilbur(websites.twitch));

                    isLinked = true;
                }
                if(input.goback){
                    AudioManager.playSound("menuchange");
                    sceneSystem.LoadSceneAsync(titleSubScene);
                    sceneSystem.UnloadScene(creditsSubScene);
                }
            }
        }).Run();
    }
}
