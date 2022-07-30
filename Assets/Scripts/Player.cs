using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    public float jumpForce = 5;
    PlayerController controller;
    Camera viewCamera;
    private Plane plane = new Plane(Vector3.up, new Vector3(0, 1, 0));
    GunController gunController;
    Rigidbody body;
    bool shootMode;

    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        body = GetComponent<Rigidbody>();
        shootMode = gunController.equipedGun.shootMode;
    }

    void Update()
    {
        // 移动输入
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //瞄准输入
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            // Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        //开枪输入
        if (shootMode) { //连发
            if (Input.GetMouseButton(0)){
                gunController.Shoot();
            }
        }
        else {  //单发
            if (Input.GetMouseButtonDown(0)){
                gunController.Shoot();
            }
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) {
            gunController.EquipLastGun();
            shootMode = gunController.equipedGun.shootMode;
        }
        else if (scroll < 0) {
            gunController.EquipNextGun();
            shootMode = gunController.equipedGun.shootMode;
        }

        //跳跃
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 1.05f) {
            body.AddForce(Vector3.up * jumpForce);
        }

        //退出游戏
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GameUI.ExitGame();
        }

        //重启游戏
        if (Input.GetKeyDown(KeyCode.F4)) {
            GameUI.StartNewGame();
        }
    }
}
