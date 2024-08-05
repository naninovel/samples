using System.Collections;
using Naninovel;
using Naninovel.E2E;
using UnityEngine.TestTools;
using static Naninovel.E2E.Shortcuts;

public class Routes
{
    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithMaxX () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonMaxX)
        .Once(Script("route-2-x/scene-1")).Ensure(Var("xPoints", 13));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithMaxY () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonMaxY)
        .Once(Script("route-2-y/scene-1")).Ensure(Var("yPoints", 12));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithMinX () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonMinX).Ensure(Var("xPoints", 3));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithMinY () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonMinY).Ensure(Var("yPoints", 3));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithBadEnd1 () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonBad1).Ensure(Var("xPoints", 2));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithBadEnd2 () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonBad2).Ensure(Var("yPoints", 2));

    [UnityTest]
    public IEnumerator CanCompleteCommonRouteWithDeadEnd () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonDead).Ensure(Var("healthPoints", 0));

    [UnityTest]
    public IEnumerator WhenXYRoutesCompleteTrueUnlocks () => new E2E()
        .WithTransientState().WithFastForward()
        .StartNew().Play(CommonMaxX, RouteX)
        .StartNew().Play(CommonMaxY, RouteY)
        .Once(InTitle).Ensure(() => UI("TrueRoute").Visible);

    [UnityTest]
    public IEnumerator WhenTrueRouteCompleteTitleBackChanges () => new E2E()
        .WithTransientState(GlobalStateMap.With(
            new CustomVariableManager.GlobalState {
                GlobalVariables = new[] {
                    new CustomVariable("g_completedX", CustomVariableScope.Global, new CustomVariableValue(true)),
                    new CustomVariable("g_completedY", CustomVariableScope.Global, new CustomVariableValue(true))
                }
            }))
        .WithFastForward()
        .Once(InTitle).Click("TrueRouteButton").Play(TrueRoute)
        .Once(InTitle).Ensure(() => MainBack.Appearance == "Snow");

    ISequence CommonMaxX => Play(D1QuickX, D2TowardX, D3LooseHP);
    ISequence CommonMaxY => Play(D1QuickY, D2TowardY, D3LooseX);
    ISequence CommonMinX => Play(D1QuickNone, D2TowardX, D3LooseHP);
    ISequence CommonMinY => Play(D1QuickNone, D2TowardY, D3LooseX, D3LastY);
    ISequence CommonBad1 => Play(D1QuickNone, D2TowardX, D3LooseX);
    ISequence CommonBad2 => Play(D1QuickNone, D2TowardY, D3LooseX, D3LastNah);
    ISequence CommonDead => Play(D1QuickNone, D2TowardY, D3LooseHP);

    ISequence D1QuickX => Once(Choice("d1-qte-x")).Choose("d1-qte-x");
    ISequence D1QuickY => Once(Choice("d1-qte-y")).Choose("d1-qte-y");
    ISequence D1QuickNone => Once(Choice()).Wait(0.5f);

    ISequence D2TowardX => Once(Choosing).Choose("d2-toward-x");
    ISequence D2TowardY => Once(Choosing).Choose("d2-toward-y");

    ISequence D3LooseHP => Once(Choosing).Choose("d3-loose-hp");
    ISequence D3LooseX => Once(Choosing).Choose("d3-loose-x");
    ISequence D3LastY => Once(Choosing).Choose("d3-last-y");
    ISequence D3LastNah => Once(Choosing).Choose("d3-last-nah");

    ISequence RouteX => On(Choosing, Choose(), Var("g_completedX", false));
    ISequence RouteY => On(Choosing, Choose(), Var("g_completedY", false));
    ISequence TrueRoute => Once(Var("g_completedTrue", true));
}
