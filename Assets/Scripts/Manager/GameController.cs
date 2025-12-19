using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Manager
{
    /// <summary>
    /// 游戏主控制器 - 连接所有系统
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("核心管理器")]
        [SerializeField] private GameFlowManager flowManager;
        [SerializeField] private PlayerAttributeManager attributeManager;

        [Header("UI面板")]
        [SerializeField] private UI.StateCardPanel stateCardPanel;
        [SerializeField] private UI.MenuPanel menuPanel;
        [SerializeField] private UI.OrderConfirmDialog orderConfirmDialog;
        [SerializeField] private UI.PlayerAttributeDisplay attributeDisplay;

        [Header("游戏数据")]
        [SerializeField] private List<FoodItemConfig> allFoodItems = new List<FoodItemConfig>();

        private FoodItemConfig pendingFood; // 待确认的食物

        private void Start()
        {
            InitializeGame();
            SubscribeToEvents();
        }

        private void InitializeGame()
        {
            // 初始化菜单
            if (menuPanel != null)
                menuPanel.InitializeMenu(allFoodItems);

            // 初始隐藏所有面板
            HideAllPanels();
        }

        private void SubscribeToEvents()
        {
            // 订阅游戏流程事件
            if (flowManager != null)
            {
                flowManager.OnStateCardPresented += OnStateCardPresented;
                flowManager.OnPhaseChanged += OnPhaseChanged;
                flowManager.OnDayEnded += OnDayEnded;
                flowManager.OnGameEnded += OnGameEnded;
            }

            // 订阅UI事件
            if (stateCardPanel != null)
            {
                stateCardPanel.OnConfirmed += OnStateCardConfirmed;
            }

            if (menuPanel != null)
            {
                menuPanel.OnFoodSelected += OnFoodSelected;
                menuPanel.OnOrderConfirmed += OnOrderConfirmed;
            }

            if (orderConfirmDialog != null)
            {
                orderConfirmDialog.OnConfirmed += OnOrderConfirmDialogConfirmed;
                orderConfirmDialog.OnCancelled += OnOrderConfirmDialogCancelled;
            }
        }

        #region 游戏流程回调

        private void OnStateCardPresented(StateCardConfig stateCard)
        {
            // 显示状态卡片
            if (stateCardPanel != null)
            {
                stateCardPanel.SetData(stateCard);
                stateCardPanel.Show();
            }
        }

        private void OnPhaseChanged(GamePhase phase)
        {
            Debug.Log($"游戏阶段切换到: {phase}");

            switch (phase)
            {
                case GamePhase.ViewStateCard:
                    // 已在OnStateCardPresented中处理
                    break;

                case GamePhase.OrderFood:
                    ShowOrderPhase();
                    break;

                case GamePhase.CheckResult:
                    ShowResultPhase();
                    break;
            }
        }

        private void OnDayEnded(bool taskCompleted, string resultText)
        {
            if (taskCompleted)
            {
                Debug.Log("任务完成，进入结算阶段");
            }
            else
            {
                Debug.Log("任务失败，进入结算阶段");
            }
            
            // 显示结果弹窗（这里可以创建专门的结果面板）
            ShowDayResultDialog(taskCompleted, resultText);
        }

        private void OnGameEnded(GameEndType endType, string endMessage)
        {
            Debug.Log($"游戏结束 - 结局类型: {endType}");
            Debug.Log($"结局消息: {endMessage}");

            // 显示游戏结束画面（这里可以创建专门的结局面板）
            ShowGameEndDialog(endType, endMessage);
        }

        #endregion

        #region UI回调

        private void OnStateCardConfirmed()
        {
            // 确认状态卡片，进入点餐阶段
            if (stateCardPanel != null)
                stateCardPanel.Hide();

            if (flowManager != null)
                flowManager.ConfirmStateCard();
        }

        private void OnFoodSelected(FoodItemConfig food)
        {
            // 选中食物，显示确认对话框
            pendingFood = food;

            if (orderConfirmDialog != null)
            {
                orderConfirmDialog.SetData(food);
                orderConfirmDialog.Show();
            }
        }

        private void OnOrderConfirmDialogConfirmed(FoodItemConfig food)
        {
            // 确认点餐
            if (flowManager != null)
                flowManager.OrderFood(food);

            if (orderConfirmDialog != null)
                orderConfirmDialog.Hide();

            pendingFood = null;

            Debug.Log($"已点餐: {food.DisplayName}");
        }

        private void OnOrderConfirmDialogCancelled()
        {
            // 取消点餐
            if (orderConfirmDialog != null)
                orderConfirmDialog.Hide();

            pendingFood = null;
        }

        private void OnOrderConfirmed()
        {
            // 确认所有点餐，进入结算
            if (menuPanel != null)
                menuPanel.Hide();

            if (flowManager != null)
                flowManager.ConfirmOrder();
        }

        #endregion

        #region 阶段显示

        private void ShowOrderPhase()
        {
            if (menuPanel != null)
                menuPanel.Show();
        }

        private void ShowResultPhase()
        {
            // 结果阶段的UI显示
        }

        private void ShowDayResultDialog(bool success, string message)
        {
            // TODO: 创建并显示当天结果对话框
            // 这里可以使用一个通用的对话框UI
            
            // 3秒后自动进入下一天
            Invoke(nameof(ProceedToNextDay), 3f);
        }

        private void ShowGameEndDialog(GameEndType endType, string message)
        {
            // TODO: 创建并显示游戏结束界面
        }

        private void ProceedToNextDay()
        {
            if (flowManager != null)
                flowManager.ProceedToNextDay();
        }

        #endregion

        private void HideAllPanels()
        {
            if (stateCardPanel != null)
                stateCardPanel.Hide(true);
            
            if (menuPanel != null)
                menuPanel.Hide(true);
            
            if (orderConfirmDialog != null)
                orderConfirmDialog.Hide(true);
        }

        private void OnDestroy()
        {
            // 取消订阅事件
            if (flowManager != null)
            {
                flowManager.OnStateCardPresented -= OnStateCardPresented;
                flowManager.OnPhaseChanged -= OnPhaseChanged;
                flowManager.OnDayEnded -= OnDayEnded;
                flowManager.OnGameEnded -= OnGameEnded;
            }

            if (stateCardPanel != null)
            {
                stateCardPanel.OnConfirmed -= OnStateCardConfirmed;
            }

            if (menuPanel != null)
            {
                menuPanel.OnFoodSelected -= OnFoodSelected;
                menuPanel.OnOrderConfirmed -= OnOrderConfirmed;
            }

            if (orderConfirmDialog != null)
            {
                orderConfirmDialog.OnConfirmed -= OnOrderConfirmDialogConfirmed;
                orderConfirmDialog.OnCancelled -= OnOrderConfirmDialogCancelled;
            }
        }
    }
}
